using Core.IntegrationTests.Helpers;
using Core.Services;

namespace Core.IntegrationTests.Services;

public class MakeServiceTests
{
    private const int HondaMakeId = 1;
    private const string HondaCode = "HONDA";
    private const string HondaName = "Honda";

    [Fact]
    public async Task GetAllAsync_EmptyDatabase_ReturnsEmptyList()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();
        var service = new MakeService(context, NullLogger<MakeService>.Instance);

        // Act
        var result = await service.GetAllAsync();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_MultipleMakes_ReturnsAllOrderedByName()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();
        context.Makes.AddRange(
            new Domain.Entities.Make { MakeId = 1, Code = "TOYOTA", Name = "Toyota" },
            new Domain.Entities.Make { MakeId = 2, Code = HondaCode, Name = HondaName },
            new Domain.Entities.Make { MakeId = 3, Code = "FORD", Name = "Ford" }
        );
        await context.SaveChangesAsync();
        var service = new MakeService(context, NullLogger<MakeService>.Instance);

        // Act
        var result = await service.GetAllAsync();

        // Assert
        Assert.Equal(3, result.Count);
        Assert.Equal(new[] { "Ford", HondaName, "Toyota" }, result.Select(m => m.Name).ToArray());
    }

    [Fact]
    public async Task GetAllAsync_SingleMake_ReturnsCorrectResponseShape()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();
        context.Makes.Add(new Domain.Entities.Make { MakeId = HondaMakeId, Code = HondaCode, Name = HondaName });
        await context.SaveChangesAsync();
        var service = new MakeService(context, NullLogger<MakeService>.Instance);

        // Act
        var result = await service.GetAllAsync();

        // Assert
        var make = Assert.Single(result);
        Assert.Equal(HondaMakeId, make.MakeId);
        Assert.Equal(HondaCode, make.Code);
        Assert.Equal(HondaName, make.Name);
    }
}
