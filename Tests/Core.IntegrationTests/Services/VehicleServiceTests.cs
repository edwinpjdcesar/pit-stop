using Core.IntegrationTests.Helpers;
using Core.Services;
using Domain.Dtos;
using Domain.Exceptions;

namespace Core.IntegrationTests.Services;

public class VehicleServiceTests
{
    private const string UpdatedVin = "1HGCM82633A999999";
    private const string UpdatedLicensePlate = "XYZ789";
    private const int UpdatedYear = 2021;
    private const int UpdatedMileage = 30000;

    [Fact]
    public async Task GetAllAsync_EmptyDatabase_ReturnsEmptyList()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();
        var service = new VehicleService(context);

        // Act
        var result = await service.GetAllAsync();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_MultipleVehicles_ReturnsOrderedByYear()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();
        await DbSeeder.SeedMakeAndModelAsync(context);
        await DbSeeder.SeedVehicleAsync(context, year: 2022);
        await DbSeeder.SeedVehicleAsync(context, year: 2018);
        await DbSeeder.SeedVehicleAsync(context, year: 2020);
        var service = new VehicleService(context);

        // Act
        var result = await service.GetAllAsync();

        // Assert
        Assert.Equal(new[] { 2018, 2020, 2022 }, result.Select(v => v.Year).ToArray());
    }

    [Fact]
    public async Task GetByIdAsync_VehicleDoesNotExist_ReturnsNull()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();
        var service = new VehicleService(context);

        // Act
        var result = await service.GetByIdAsync(Guid.NewGuid());

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_VehicleExists_ReturnsCorrectVehicle()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();
        await DbSeeder.SeedMakeAndModelAsync(context);
        var vehicle = await DbSeeder.SeedVehicleAsync(context);
        var service = new VehicleService(context);

        // Act
        var result = await service.GetByIdAsync(vehicle.VehicleId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(vehicle.VehicleId, result.VehicleId);
        Assert.Equal(2020, result.Year);
        Assert.Equal("HONDA", result.Make.Code);
        Assert.Equal("CIVIC", result.Model.Code);
    }

    [Fact]
    public async Task CreateAsync_MakeDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();
        var service = new VehicleService(context);
        var request = new VehicleRequest(MakeId: 99, ModelId: 1, VIN: null, LicensePlate: null,
            Year: 2020, PurchaseDate: null, PurchasePrice: null, MileageAtPurchase: null, Mileage: null);

        // Act
        var ex = await Assert.ThrowsAsync<NotFoundException>(() => service.CreateAsync(request));

        // Assert
        Assert.Contains("Make", ex.Message);
    }

    [Fact]
    public async Task CreateAsync_ModelDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();
        await DbSeeder.SeedMakeAndModelAsync(context);
        var service = new VehicleService(context);
        var request = new VehicleRequest(MakeId: 1, ModelId: 99, VIN: null, LicensePlate: null,
            Year: 2020, PurchaseDate: null, PurchasePrice: null, MileageAtPurchase: null, Mileage: null);

        // Act
        var ex = await Assert.ThrowsAsync<NotFoundException>(() => service.CreateAsync(request));

        // Assert
        Assert.Contains("Model", ex.Message);
    }

    [Fact]
    public async Task CreateAsync_ModelBelongsToDifferentMake_ThrowsConflictException()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();
        await DbSeeder.SeedMakeAndModelAsync(context);
        context.Makes.Add(new Domain.Entities.Make { MakeId = 2, Code = "TOYOTA", Name = "Toyota" });
        context.Models.Add(new Domain.Entities.Model { ModelId = 2, MakeId = 2, Code = "CAMRY", Name = "Camry" });
        await context.SaveChangesAsync();
        var service = new VehicleService(context);
        var request = new VehicleRequest(MakeId: 1, ModelId: 2, VIN: null, LicensePlate: null,
            Year: 2020, PurchaseDate: null, PurchasePrice: null, MileageAtPurchase: null, Mileage: null);

        // Act
        var ex = await Assert.ThrowsAsync<ConflictException>(() => service.CreateAsync(request));

        // Assert
        Assert.Contains("Model", ex.Message);
    }

    [Fact]
    public async Task CreateAsync_ValidRequest_ReturnsCreatedVehicle()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();
        await DbSeeder.SeedMakeAndModelAsync(context);
        var service = new VehicleService(context);
        var request = new VehicleRequest(MakeId: 1, ModelId: 1, VIN: "1HGCM82633A123456",
            LicensePlate: "ABC123", Year: 2021, PurchaseDate: new DateOnly(2023, 1, 15),
            PurchasePrice: 22000m, MileageAtPurchase: 0, Mileage: 5000);

        // Act
        var result = await service.CreateAsync(request);

        // Assert
        Assert.NotEqual(Guid.Empty, result.VehicleId);
        Assert.Equal(2021, result.Year);
        Assert.Equal("HONDA", result.Make.Code);
        Assert.Equal("CIVIC", result.Model.Code);
    }

    [Fact]
    public async Task CreateAsync_ValidRequest_PersistsVehicleToDatabase()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();
        await DbSeeder.SeedMakeAndModelAsync(context);
        var service = new VehicleService(context);
        var request = new VehicleRequest(MakeId: 1, ModelId: 1, VIN: null, LicensePlate: null,
            Year: 2020, PurchaseDate: null, PurchasePrice: null, MileageAtPurchase: null, Mileage: null);

        // Act
        await service.CreateAsync(request);

        // Assert
        Assert.Single(context.Vehicles);
    }

    [Fact]
    public async Task UpdateAsync_ModelBelongsToDifferentMake_ThrowsConflictException()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();
        await DbSeeder.SeedMakeAndModelAsync(context);
        var vehicle = await DbSeeder.SeedVehicleAsync(context);
        context.Makes.Add(new Domain.Entities.Make { MakeId = 2, Code = "TOYOTA", Name = "Toyota" });
        context.Models.Add(new Domain.Entities.Model { ModelId = 2, MakeId = 2, Code = "CAMRY", Name = "Camry" });
        await context.SaveChangesAsync();
        var service = new VehicleService(context);
        var request = new VehicleRequest(MakeId: 1, ModelId: 2, VIN: null, LicensePlate: null,
            Year: 2020, PurchaseDate: null, PurchasePrice: null, MileageAtPurchase: null, Mileage: null);

        // Act
        var ex = await Assert.ThrowsAsync<ConflictException>(() => service.UpdateAsync(vehicle.VehicleId, request));

        // Assert
        Assert.Contains("Model", ex.Message);
    }

    [Fact]
    public async Task UpdateAsync_VehicleDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();
        await DbSeeder.SeedMakeAndModelAsync(context);
        var service = new VehicleService(context);
        var request = new VehicleRequest(MakeId: 1, ModelId: 1, VIN: null, LicensePlate: null,
            Year: 2020, PurchaseDate: null, PurchasePrice: null, MileageAtPurchase: null, Mileage: null);

        // Act
        var ex = await Assert.ThrowsAsync<NotFoundException>(() => service.UpdateAsync(Guid.NewGuid(), request));

        // Assert
        Assert.Contains("Vehicle", ex.Message);
    }

    [Fact]
    public async Task UpdateAsync_ValidRequest_ReturnsUpdatedVehicle()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();
        await DbSeeder.SeedMakeAndModelAsync(context);
        var vehicle = await DbSeeder.SeedVehicleAsync(context);
        var service = new VehicleService(context);
        var request = new VehicleRequest(MakeId: 1, ModelId: 1, VIN: UpdatedVin,
            LicensePlate: UpdatedLicensePlate, Year: UpdatedYear,
            PurchaseDate: null, PurchasePrice: null, MileageAtPurchase: null, Mileage: UpdatedMileage);

        // Act
        var result = await service.UpdateAsync(vehicle.VehicleId, request);

        // Assert
        Assert.Equal(UpdatedVin, result.VIN);
        Assert.Equal(UpdatedLicensePlate, result.LicensePlate);
        Assert.Equal(UpdatedYear, result.Year);
        Assert.Equal(UpdatedMileage, result.Mileage);
    }

    [Fact]
    public async Task DeleteAsync_VehicleDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();
        var service = new VehicleService(context);

        // Act
        var ex = await Assert.ThrowsAsync<NotFoundException>(() => service.DeleteAsync(Guid.NewGuid()));

        // Assert
        Assert.Contains("Vehicle", ex.Message);
    }

    [Fact]
    public async Task DeleteAsync_VehicleExists_RemovesVehicleFromDatabase()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();
        await DbSeeder.SeedMakeAndModelAsync(context);
        var vehicle = await DbSeeder.SeedVehicleAsync(context);
        var service = new VehicleService(context);

        // Act
        await service.DeleteAsync(vehicle.VehicleId);

        // Assert
        Assert.Empty(context.Vehicles);
    }
}
