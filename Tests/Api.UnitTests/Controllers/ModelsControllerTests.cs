using Api.Controllers;
using Core.Services.Interfaces;
using Domain.Dtos;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace Api.UnitTests.Controllers;

public class ModelsControllerTests
{
    private readonly IModelService _modelService = Substitute.For<IModelService>();

    private ModelsController CreateController() => new(_modelService);

    [Fact]
    public async Task GetByMake_WhenCalled_ReturnsOkWithModels()
    {
        // Arrange
        const int MakeId = 1;
        IReadOnlyList<ModelResponse> models = [new(10, MakeId, "Ford", "F150", "F-150")];
        _modelService.GetByMakeAsync(MakeId, Arg.Any<CancellationToken>()).Returns(models);
        var controller = CreateController();

        // Act
        var result = await controller.GetByMake(MakeId, CancellationToken.None);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(models, ok.Value);
    }
}
