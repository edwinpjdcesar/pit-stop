using Api.Controllers;
using Core.Services.Interfaces;
using Domain.Dtos;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace Api.UnitTests.Controllers;

public class VehiclesControllerTests
{
    private readonly IVehicleService _vehicleService = Substitute.For<IVehicleService>();

    private static readonly MakeResponse Make = new(1, "FORD", "Ford");
    private static readonly ModelResponse Model = new(10, 1, "Ford", "F150", "F-150");

    private static VehicleResponse BuildVehicleResponse(Guid vehicleId) =>
        new(vehicleId, null, null, 2020, Make, Model, null, null, null, null);

    private VehiclesController CreateController() => new(_vehicleService);

    [Fact]
    public async Task GetAll_WhenCalled_ReturnsOkWithVehicles()
    {
        // Arrange
        IReadOnlyList<VehicleResponse> vehicles = [BuildVehicleResponse(Guid.NewGuid())];
        _vehicleService.GetAllAsync(Arg.Any<CancellationToken>()).Returns(vehicles);
        var controller = CreateController();

        // Act
        var result = await controller.GetAll(CancellationToken.None);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(vehicles, ok.Value);
    }

    [Fact]
    public async Task GetById_VehicleExists_ReturnsOkWithVehicle()
    {
        // Arrange
        var vehicleId = Guid.NewGuid();
        var vehicle = BuildVehicleResponse(vehicleId);
        _vehicleService.GetByIdAsync(vehicleId, Arg.Any<CancellationToken>()).Returns(vehicle);
        var controller = CreateController();

        // Act
        var result = await controller.GetById(vehicleId, CancellationToken.None);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(vehicle, ok.Value);
    }

    [Fact]
    public async Task GetById_VehicleDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var vehicleId = Guid.NewGuid();
        _vehicleService.GetByIdAsync(vehicleId, Arg.Any<CancellationToken>()).Returns((VehicleResponse?)null);
        var controller = CreateController();

        // Act
        var result = await controller.GetById(vehicleId, CancellationToken.None);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Create_WhenCalled_ReturnsCreatedAtAction()
    {
        // Arrange
        var vehicleId = Guid.NewGuid();
        var request = new VehicleRequest(1, 10, null, null, 2020, null, null, null, null);
        var vehicle = BuildVehicleResponse(vehicleId);
        _vehicleService.CreateAsync(request, Arg.Any<CancellationToken>()).Returns(vehicle);
        var controller = CreateController();

        // Act
        var result = await controller.Create(request, CancellationToken.None);

        // Assert
        var created = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(vehicle, created.Value);
        Assert.Equal(nameof(controller.GetById), created.ActionName);
    }

    [Fact]
    public async Task Update_WhenCalled_ReturnsOkWithUpdatedVehicle()
    {
        // Arrange
        var vehicleId = Guid.NewGuid();
        var request = new VehicleRequest(1, 10, null, null, 2021, null, null, null, null);
        var vehicle = BuildVehicleResponse(vehicleId);
        _vehicleService.UpdateAsync(vehicleId, request, Arg.Any<CancellationToken>()).Returns(vehicle);
        var controller = CreateController();

        // Act
        var result = await controller.Update(vehicleId, request, CancellationToken.None);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(vehicle, ok.Value);
    }

    [Fact]
    public async Task Delete_WhenCalled_ReturnsNoContent()
    {
        // Arrange
        var vehicleId = Guid.NewGuid();
        var controller = CreateController();

        // Act
        var result = await controller.Delete(vehicleId, CancellationToken.None);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }
}
