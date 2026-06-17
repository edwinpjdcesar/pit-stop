using Core.IntegrationTests.Helpers;
using Core.Services;
using Domain.Dtos;
using Domain.Exceptions;

namespace Core.IntegrationTests.Services;

public class MaintenanceServiceTests
{
    private const string OilChangeDescription = "Oil change";
    private const int ServiceMileage = 24000;
    private static readonly DateTime ServiceDate = new(2026, 5, 1, 10, 30, 0, DateTimeKind.Utc);

    private const string UpdatedDescription = "Oil change and tire rotation";
    private const int UpdatedMileage = 25000;
    private static readonly DateTime UpdatedServiceDate = new(2026, 6, 15, 0, 0, 0, DateTimeKind.Utc);

    [Fact]
    public async Task GetByVehicleAsync_VehicleDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();
        var service = new MaintenanceService(context);

        // Act
        var ex = await Assert.ThrowsAsync<NotFoundException>(() => service.GetByVehicleAsync(Guid.NewGuid()));

        // Assert
        Assert.Contains("Vehicle", ex.Message);
    }

    [Fact]
    public async Task GetByVehicleAsync_VehicleHasNoMaintenance_ReturnsEmptyList()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();
        await DbSeeder.SeedMakeAndModelAsync(context);
        var vehicle = await DbSeeder.SeedVehicleAsync(context);
        var service = new MaintenanceService(context);

        // Act
        var result = await service.GetByVehicleAsync(vehicle.VehicleId);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByVehicleAsync_MultipleRecords_ReturnsOrderedByServiceDateDescending()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();
        await DbSeeder.SeedMakeAndModelAsync(context);
        var vehicle = await DbSeeder.SeedVehicleAsync(context);
        context.MaintenanceRecords.AddRange(
            new Domain.Entities.Maintenance { MaintenanceId = Guid.NewGuid(), VehicleId = vehicle.VehicleId, Description = "Older service", Mileage = 10000, ServiceDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Domain.Entities.Maintenance { MaintenanceId = Guid.NewGuid(), VehicleId = vehicle.VehicleId, Description = "Newer service", Mileage = 20000, ServiceDate = new DateTime(2025, 6, 1, 0, 0, 0, DateTimeKind.Utc) }
        );
        await context.SaveChangesAsync();
        var service = new MaintenanceService(context);

        // Act
        var result = await service.GetByVehicleAsync(vehicle.VehicleId);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("Newer service", result[0].Description);
        Assert.Equal("Older service", result[1].Description);
    }

    [Fact]
    public async Task AddAsync_VehicleDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();
        var service = new MaintenanceService(context);
        var request = new MaintenanceRequest(OilChangeDescription, ServiceMileage, ServiceDate);

        // Act
        var ex = await Assert.ThrowsAsync<NotFoundException>(() => service.AddAsync(Guid.NewGuid(), request));

        // Assert
        Assert.Contains("Vehicle", ex.Message);
    }

    [Fact]
    public async Task AddAsync_ValidRequest_ReturnsCreatedMaintenanceRecord()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();
        await DbSeeder.SeedMakeAndModelAsync(context);
        var vehicle = await DbSeeder.SeedVehicleAsync(context);
        var service = new MaintenanceService(context);
        var request = new MaintenanceRequest(OilChangeDescription, ServiceMileage, ServiceDate);

        // Act
        var result = await service.AddAsync(vehicle.VehicleId, request);

        // Assert
        Assert.NotEqual(Guid.Empty, result.MaintenanceId);
        Assert.Equal(vehicle.VehicleId, result.VehicleId);
        Assert.Equal(OilChangeDescription, result.Description);
        Assert.Equal(ServiceMileage, result.Mileage);
        Assert.Equal(ServiceDate, result.ServiceDate);
        Assert.Empty(result.MaintenanceParts);
    }

    [Fact]
    public async Task AddAsync_ValidRequest_PersistsRecordToDatabase()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();
        await DbSeeder.SeedMakeAndModelAsync(context);
        var vehicle = await DbSeeder.SeedVehicleAsync(context);
        var service = new MaintenanceService(context);
        var request = new MaintenanceRequest(OilChangeDescription, ServiceMileage, ServiceDate);

        // Act
        await service.AddAsync(vehicle.VehicleId, request);

        // Assert
        Assert.Single(context.MaintenanceRecords);
    }

    [Fact]
    public async Task UpdateAsync_MaintenanceNotFoundForVehicle_ThrowsNotFoundException()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();
        await DbSeeder.SeedMakeAndModelAsync(context);
        var vehicle = await DbSeeder.SeedVehicleAsync(context);
        var service = new MaintenanceService(context);
        var request = new MaintenanceRequest(UpdatedDescription, UpdatedMileage, UpdatedServiceDate);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => service.UpdateAsync(vehicle.VehicleId, Guid.NewGuid(), request));
    }

    [Fact]
    public async Task UpdateAsync_MaintenanceBelongsToDifferentVehicle_ThrowsNotFoundException()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();
        await DbSeeder.SeedMakeAndModelAsync(context);
        var vehicle1 = await DbSeeder.SeedVehicleAsync(context);
        var vehicle2 = await DbSeeder.SeedVehicleAsync(context);
        var maintenance = await DbSeeder.SeedMaintenanceAsync(context, vehicle1.VehicleId);
        var service = new MaintenanceService(context);
        var request = new MaintenanceRequest(UpdatedDescription, UpdatedMileage, UpdatedServiceDate);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => service.UpdateAsync(vehicle2.VehicleId, maintenance.MaintenanceId, request));
    }

    [Fact]
    public async Task UpdateAsync_ValidRequest_ReturnsUpdatedRecord()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();
        await DbSeeder.SeedMakeAndModelAsync(context);
        var vehicle = await DbSeeder.SeedVehicleAsync(context);
        var maintenance = await DbSeeder.SeedMaintenanceAsync(context, vehicle.VehicleId);
        var service = new MaintenanceService(context);
        var request = new MaintenanceRequest(UpdatedDescription, UpdatedMileage, UpdatedServiceDate);

        // Act
        var result = await service.UpdateAsync(vehicle.VehicleId, maintenance.MaintenanceId, request);

        // Assert
        Assert.Equal(UpdatedDescription, result.Description);
        Assert.Equal(UpdatedMileage, result.Mileage);
        Assert.Equal(UpdatedServiceDate, result.ServiceDate);
    }

    [Fact]
    public async Task DeleteAsync_MaintenanceNotFoundForVehicle_ThrowsNotFoundException()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();
        await DbSeeder.SeedMakeAndModelAsync(context);
        var vehicle = await DbSeeder.SeedVehicleAsync(context);
        var service = new MaintenanceService(context);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => service.DeleteAsync(vehicle.VehicleId, Guid.NewGuid()));
    }

    [Fact]
    public async Task DeleteAsync_ValidIds_RemovesRecordFromDatabase()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();
        await DbSeeder.SeedMakeAndModelAsync(context);
        var vehicle = await DbSeeder.SeedVehicleAsync(context);
        var maintenance = await DbSeeder.SeedMaintenanceAsync(context, vehicle.VehicleId);
        var service = new MaintenanceService(context);

        // Act
        await service.DeleteAsync(vehicle.VehicleId, maintenance.MaintenanceId);

        // Assert
        Assert.Empty(context.MaintenanceRecords);
    }

    [Fact]
    public async Task ValidateOwnershipAsync_MaintenanceDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();
        await DbSeeder.SeedMakeAndModelAsync(context);
        var vehicle = await DbSeeder.SeedVehicleAsync(context);
        var service = new MaintenanceService(context);

        // Act
        var ex = await Assert.ThrowsAsync<NotFoundException>(() =>
            service.ValidateOwnershipAsync(vehicle.VehicleId, Guid.NewGuid()));

        // Assert
        Assert.Contains("Maintenance", ex.Message);
    }

    [Fact]
    public async Task ValidateOwnershipAsync_MaintenanceBelongsToDifferentVehicle_ThrowsConflictException()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();
        await DbSeeder.SeedMakeAndModelAsync(context);
        var vehicle1 = await DbSeeder.SeedVehicleAsync(context);
        var vehicle2 = await DbSeeder.SeedVehicleAsync(context);
        var maintenance = await DbSeeder.SeedMaintenanceAsync(context, vehicle1.VehicleId);
        var service = new MaintenanceService(context);

        // Act
        var ex = await Assert.ThrowsAsync<ConflictException>(() =>
            service.ValidateOwnershipAsync(vehicle2.VehicleId, maintenance.MaintenanceId));

        // Assert
        Assert.Contains(maintenance.MaintenanceId.ToString(), ex.Message);
    }

    [Fact]
    public async Task ValidateOwnershipAsync_MaintenanceBelongsToVehicle_CompletesSuccessfully()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();
        await DbSeeder.SeedMakeAndModelAsync(context);
        var vehicle = await DbSeeder.SeedVehicleAsync(context);
        var maintenance = await DbSeeder.SeedMaintenanceAsync(context, vehicle.VehicleId);
        var service = new MaintenanceService(context);

        // Act
        var ex = await Record.ExceptionAsync(() =>
            service.ValidateOwnershipAsync(vehicle.VehicleId, maintenance.MaintenanceId));

        // Assert
        Assert.Null(ex);
    }
}
