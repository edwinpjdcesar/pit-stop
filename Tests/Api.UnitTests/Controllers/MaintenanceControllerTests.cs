using Api.Controllers;
using Core.Services.Interfaces;
using Domain.Dtos;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace Api.UnitTests.Controllers;

public class MaintenanceControllerTests
{
    private readonly IMaintenanceService _maintenanceService = Substitute.For<IMaintenanceService>();

    private static MaintenanceResponse BuildMaintenanceResponse(Guid maintenanceId, Guid vehicleId) =>
        new(maintenanceId, vehicleId, "Oil change", 50000, DateTime.UtcNow, []);

    private MaintenanceController CreateController() => new(_maintenanceService);

    [Fact]
    public async Task GetByVehicle_WhenCalled_ReturnsOkWithRecords()
    {
        // Arrange
        var vehicleId = Guid.NewGuid();
        IReadOnlyList<MaintenanceResponse> records = [BuildMaintenanceResponse(Guid.NewGuid(), vehicleId)];
        _maintenanceService.GetByVehicleAsync(vehicleId, Arg.Any<CancellationToken>()).Returns(records);
        var controller = CreateController();

        // Act
        var result = await controller.GetByVehicle(vehicleId, CancellationToken.None);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(records, ok.Value);
    }

    [Fact]
    public async Task Add_WhenCalled_ReturnsCreatedAtAction()
    {
        // Arrange
        var vehicleId = Guid.NewGuid();
        var maintenanceId = Guid.NewGuid();
        var request = new MaintenanceRequest("Oil change", 50000, DateTime.UtcNow);
        var maintenance = BuildMaintenanceResponse(maintenanceId, vehicleId);
        _maintenanceService.AddAsync(vehicleId, request, Arg.Any<CancellationToken>()).Returns(maintenance);
        var controller = CreateController();

        // Act
        var result = await controller.Add(vehicleId, request, CancellationToken.None);

        // Assert
        var created = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(maintenance, created.Value);
        Assert.Equal(nameof(controller.GetByVehicle), created.ActionName);
    }

    [Fact]
    public async Task Update_WhenCalled_ReturnsOkWithUpdatedRecord()
    {
        // Arrange
        var vehicleId = Guid.NewGuid();
        var maintenanceId = Guid.NewGuid();
        var request = new MaintenanceRequest("Tire rotation", 51000, DateTime.UtcNow);
        var maintenance = BuildMaintenanceResponse(maintenanceId, vehicleId);
        _maintenanceService.UpdateAsync(vehicleId, maintenanceId, request, Arg.Any<CancellationToken>()).Returns(maintenance);
        var controller = CreateController();

        // Act
        var result = await controller.Update(vehicleId, maintenanceId, request, CancellationToken.None);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(maintenance, ok.Value);
    }

    [Fact]
    public async Task Delete_WhenCalled_ReturnsNoContent()
    {
        // Arrange
        var vehicleId = Guid.NewGuid();
        var maintenanceId = Guid.NewGuid();
        var controller = CreateController();

        // Act
        var result = await controller.Delete(vehicleId, maintenanceId, CancellationToken.None);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }
}
