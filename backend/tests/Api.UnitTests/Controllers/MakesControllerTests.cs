using Api.Controllers;
using Core.Services.Interfaces;
using Domain.Dtos;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace Api.UnitTests.Controllers;

public class MakesControllerTests
{
    private readonly IMakeService _makeService = Substitute.For<IMakeService>();

    private MakesController CreateController() => new(_makeService);

    [Fact]
    public async Task GetAll_WhenCalled_ReturnsOkWithMakes()
    {
        // Arrange
        IReadOnlyList<MakeResponse> makes = [new(1, "FORD", "Ford"), new(2, "TOYOTA", "Toyota")];
        _makeService.GetAllAsync(Arg.Any<CancellationToken>()).Returns(makes);
        var controller = CreateController();

        // Act
        var result = await controller.GetAll(CancellationToken.None);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(makes, ok.Value);
    }
}
