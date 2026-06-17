using Api.Controllers;
using Core.Services.Interfaces;
using Domain.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace Api.UnitTests.Controllers;

public class MaintenancePartsControllerTests
{
    private readonly IMaintenancePartService _maintenancePartService = Substitute.For<IMaintenancePartService>();
    private readonly IMaintenanceService _maintenanceService = Substitute.For<IMaintenanceService>();

    private static MaintenancePartResponse BuildMaintenancePartResponse(Guid maintenanceId, Guid partId) =>
        new(maintenanceId, partId, "Oil Filter", 1, 12.99m);

    private MaintenancePartsController CreateController() =>
        new(_maintenancePartService, _maintenanceService);

    [Fact]
    public async Task AddPart_WhenCalled_ReturnsCreated()
    {
        // Arrange
        var vehicleId = Guid.NewGuid();
        var maintenanceId = Guid.NewGuid();
        var partId = Guid.NewGuid();
        var request = new MaintenancePartRequest(1, 12.99m);
        var response = BuildMaintenancePartResponse(maintenanceId, partId);
        _maintenancePartService.AddPartAsync(maintenanceId, partId, request, Arg.Any<CancellationToken>()).Returns(response);
        var controller = CreateController();

        // Act
        var result = await controller.AddPart(vehicleId, maintenanceId, partId, request, CancellationToken.None);

        // Assert
        var statusCode = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status201Created, statusCode.StatusCode);
        Assert.Equal(response, statusCode.Value);
    }

    [Fact]
    public async Task UpdatePart_WhenCalled_ReturnsOkWithUpdatedLink()
    {
        // Arrange
        var vehicleId = Guid.NewGuid();
        var maintenanceId = Guid.NewGuid();
        var partId = Guid.NewGuid();
        var request = new MaintenancePartRequest(2, 14.99m);
        var response = BuildMaintenancePartResponse(maintenanceId, partId);
        _maintenancePartService.UpdatePartAsync(maintenanceId, partId, request, Arg.Any<CancellationToken>()).Returns(response);
        var controller = CreateController();

        // Act
        var result = await controller.UpdatePart(vehicleId, maintenanceId, partId, request, CancellationToken.None);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(response, ok.Value);
    }

    [Fact]
    public async Task RemovePart_WhenCalled_ReturnsNoContent()
    {
        // Arrange
        var vehicleId = Guid.NewGuid();
        var maintenanceId = Guid.NewGuid();
        var partId = Guid.NewGuid();
        var controller = CreateController();

        // Act
        var result = await controller.RemovePart(vehicleId, maintenanceId, partId, CancellationToken.None);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }
}
