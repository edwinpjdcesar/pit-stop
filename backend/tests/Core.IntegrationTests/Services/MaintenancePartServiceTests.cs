using Core.IntegrationTests.Helpers;
using Core.Services;
using Domain.Dtos;
using Domain.Exceptions;

namespace Core.IntegrationTests.Services;

public class MaintenancePartServiceTests
{
    private const int DefaultQuantity = 2;
    private const decimal DefaultUnitPrice = 12.99m;
    private const int UpdatedQuantity = 3;
    private const decimal UpdatedUnitPrice = 9.50m;
    private const string OilFilterName = "Oil Filter";

    [Fact]
    public async Task AddPartAsync_MaintenanceDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();
        var part = await DbSeeder.SeedPartAsync(context);
        var service = new MaintenancePartService(context, NullLogger<MaintenancePartService>.Instance);
        var request = new MaintenancePartRequest(Quantity: 1, UnitPrice: DefaultUnitPrice);

        // Act
        var ex = await Assert.ThrowsAsync<NotFoundException>(() => service.AddPartAsync(Guid.NewGuid(), part.PartId, request));

        // Assert
        Assert.Contains("Maintenance", ex.Message);
    }

    [Fact]
    public async Task AddPartAsync_PartDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();
        await DbSeeder.SeedMakeAndModelAsync(context);
        var vehicle = await DbSeeder.SeedVehicleAsync(context);
        var maintenance = await DbSeeder.SeedMaintenanceAsync(context, vehicle.VehicleId);
        var service = new MaintenancePartService(context, NullLogger<MaintenancePartService>.Instance);
        var request = new MaintenancePartRequest(Quantity: 1, UnitPrice: DefaultUnitPrice);

        // Act
        var ex = await Assert.ThrowsAsync<NotFoundException>(() => service.AddPartAsync(maintenance.MaintenanceId, Guid.NewGuid(), request));

        // Assert
        Assert.Contains("Part", ex.Message);
    }

    [Fact]
    public async Task AddPartAsync_PartAlreadyLinked_ThrowsInvalidOperationException()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();
        await DbSeeder.SeedMakeAndModelAsync(context);
        var vehicle = await DbSeeder.SeedVehicleAsync(context);
        var maintenance = await DbSeeder.SeedMaintenanceAsync(context, vehicle.VehicleId);
        var part = await DbSeeder.SeedPartAsync(context);
        var service = new MaintenancePartService(context, NullLogger<MaintenancePartService>.Instance);
        var request = new MaintenancePartRequest(Quantity: 1, UnitPrice: DefaultUnitPrice);
        await service.AddPartAsync(maintenance.MaintenanceId, part.PartId, request);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => service.AddPartAsync(maintenance.MaintenanceId, part.PartId, request));
    }

    [Fact]
    public async Task AddPartAsync_ValidRequest_ReturnsCreatedLink()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();
        await DbSeeder.SeedMakeAndModelAsync(context);
        var vehicle = await DbSeeder.SeedVehicleAsync(context);
        var maintenance = await DbSeeder.SeedMaintenanceAsync(context, vehicle.VehicleId);
        var part = await DbSeeder.SeedPartAsync(context);
        var service = new MaintenancePartService(context, NullLogger<MaintenancePartService>.Instance);
        var request = new MaintenancePartRequest(Quantity: DefaultQuantity, UnitPrice: DefaultUnitPrice);

        // Act
        var result = await service.AddPartAsync(maintenance.MaintenanceId, part.PartId, request);

        // Assert
        Assert.Equal(maintenance.MaintenanceId, result.MaintenanceId);
        Assert.Equal(part.PartId, result.PartId);
        Assert.Equal(OilFilterName, result.PartName);
        Assert.Equal(DefaultQuantity, result.Quantity);
        Assert.Equal(DefaultUnitPrice, result.UnitPrice);
    }

    [Fact]
    public async Task AddPartAsync_ValidRequest_PersistsLinkToDatabase()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();
        await DbSeeder.SeedMakeAndModelAsync(context);
        var vehicle = await DbSeeder.SeedVehicleAsync(context);
        var maintenance = await DbSeeder.SeedMaintenanceAsync(context, vehicle.VehicleId);
        var part = await DbSeeder.SeedPartAsync(context);
        var service = new MaintenancePartService(context, NullLogger<MaintenancePartService>.Instance);
        var request = new MaintenancePartRequest(Quantity: 1, UnitPrice: DefaultUnitPrice);

        // Act
        await service.AddPartAsync(maintenance.MaintenanceId, part.PartId, request);

        // Assert
        Assert.Single(context.MaintenanceParts);
    }

    [Fact]
    public async Task UpdatePartAsync_LinkDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();
        var service = new MaintenancePartService(context, NullLogger<MaintenancePartService>.Instance);
        var request = new MaintenancePartRequest(Quantity: UpdatedQuantity, UnitPrice: UpdatedUnitPrice);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => service.UpdatePartAsync(Guid.NewGuid(), Guid.NewGuid(), request));
    }

    [Fact]
    public async Task UpdatePartAsync_ValidRequest_ReturnsUpdatedLink()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();
        await DbSeeder.SeedMakeAndModelAsync(context);
        var vehicle = await DbSeeder.SeedVehicleAsync(context);
        var maintenance = await DbSeeder.SeedMaintenanceAsync(context, vehicle.VehicleId);
        var part = await DbSeeder.SeedPartAsync(context);
        var service = new MaintenancePartService(context, NullLogger<MaintenancePartService>.Instance);
        await service.AddPartAsync(maintenance.MaintenanceId, part.PartId, new MaintenancePartRequest(1, DefaultUnitPrice));
        var updateRequest = new MaintenancePartRequest(Quantity: UpdatedQuantity, UnitPrice: UpdatedUnitPrice);

        // Act
        var result = await service.UpdatePartAsync(maintenance.MaintenanceId, part.PartId, updateRequest);

        // Assert
        Assert.Equal(UpdatedQuantity, result.Quantity);
        Assert.Equal(UpdatedUnitPrice, result.UnitPrice);
    }

    [Fact]
    public async Task RemovePartAsync_LinkDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();
        var service = new MaintenancePartService(context, NullLogger<MaintenancePartService>.Instance);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => service.RemovePartAsync(Guid.NewGuid(), Guid.NewGuid()));
    }

    [Fact]
    public async Task RemovePartAsync_ValidIds_RemovesLinkFromDatabase()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();
        await DbSeeder.SeedMakeAndModelAsync(context);
        var vehicle = await DbSeeder.SeedVehicleAsync(context);
        var maintenance = await DbSeeder.SeedMaintenanceAsync(context, vehicle.VehicleId);
        var part = await DbSeeder.SeedPartAsync(context);
        var service = new MaintenancePartService(context, NullLogger<MaintenancePartService>.Instance);
        await service.AddPartAsync(maintenance.MaintenanceId, part.PartId, new MaintenancePartRequest(1, DefaultUnitPrice));

        // Act
        await service.RemovePartAsync(maintenance.MaintenanceId, part.PartId);

        // Assert
        Assert.Empty(context.MaintenanceParts);
    }

    [Fact]
    public async Task RemovePartAsync_ValidIds_DoesNotDeletePart()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();
        await DbSeeder.SeedMakeAndModelAsync(context);
        var vehicle = await DbSeeder.SeedVehicleAsync(context);
        var maintenance = await DbSeeder.SeedMaintenanceAsync(context, vehicle.VehicleId);
        var part = await DbSeeder.SeedPartAsync(context);
        var service = new MaintenancePartService(context, NullLogger<MaintenancePartService>.Instance);
        await service.AddPartAsync(maintenance.MaintenanceId, part.PartId, new MaintenancePartRequest(1, DefaultUnitPrice));

        // Act
        await service.RemovePartAsync(maintenance.MaintenanceId, part.PartId);

        // Assert
        Assert.Single(context.Parts);
    }
}
