using Api.Controllers;
using Core.Services.Interfaces;
using Domain.Dtos;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace Api.UnitTests.Controllers;

public class PartsControllerTests
{
    private readonly IPartService _partService = Substitute.For<IPartService>();

    private static PartResponse BuildPartResponse(Guid partId) =>
        new(partId, "Oil Filter", "OIL-001", "Standard oil filter");

    private PartsController CreateController() => new(_partService);

    [Fact]
    public async Task GetAll_WhenCalled_ReturnsOkWithParts()
    {
        // Arrange
        IReadOnlyList<PartResponse> parts = [BuildPartResponse(Guid.NewGuid())];
        _partService.GetAllAsync(Arg.Any<CancellationToken>()).Returns(parts);
        var controller = CreateController();

        // Act
        var result = await controller.GetAll(CancellationToken.None);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(parts, ok.Value);
    }

    [Fact]
    public async Task GetById_PartExists_ReturnsOkWithPart()
    {
        // Arrange
        var partId = Guid.NewGuid();
        var part = BuildPartResponse(partId);
        _partService.GetByIdAsync(partId, Arg.Any<CancellationToken>()).Returns(part);
        var controller = CreateController();

        // Act
        var result = await controller.GetById(partId, CancellationToken.None);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(part, ok.Value);
    }

    [Fact]
    public async Task GetById_PartDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var partId = Guid.NewGuid();
        _partService.GetByIdAsync(partId, Arg.Any<CancellationToken>()).Returns((PartResponse?)null);
        var controller = CreateController();

        // Act
        var result = await controller.GetById(partId, CancellationToken.None);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Create_WhenCalled_ReturnsCreatedAtAction()
    {
        // Arrange
        var partId = Guid.NewGuid();
        var request = new PartRequest("Oil Filter", "OIL-001", "Standard oil filter");
        var part = BuildPartResponse(partId);
        _partService.CreateAsync(request, Arg.Any<CancellationToken>()).Returns(part);
        var controller = CreateController();

        // Act
        var result = await controller.Create(request, CancellationToken.None);

        // Assert
        var created = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(part, created.Value);
        Assert.Equal(nameof(controller.GetById), created.ActionName);
    }

    [Fact]
    public async Task Update_WhenCalled_ReturnsOkWithUpdatedPart()
    {
        // Arrange
        var partId = Guid.NewGuid();
        var request = new PartRequest("Oil Filter Pro", "OIL-002", "Premium oil filter");
        var part = BuildPartResponse(partId);
        _partService.UpdateAsync(partId, request, Arg.Any<CancellationToken>()).Returns(part);
        var controller = CreateController();

        // Act
        var result = await controller.Update(partId, request, CancellationToken.None);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(part, ok.Value);
    }

    [Fact]
    public async Task Delete_WhenCalled_ReturnsNoContent()
    {
        // Arrange
        var partId = Guid.NewGuid();
        var controller = CreateController();

        // Act
        var result = await controller.Delete(partId, CancellationToken.None);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }
}
