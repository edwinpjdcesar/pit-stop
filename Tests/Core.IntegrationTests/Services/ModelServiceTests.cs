using Core.IntegrationTests.Helpers;
using Core.Services;
using Domain.Exceptions;

namespace Core.IntegrationTests.Services;

public class ModelServiceTests
{
    private const int HondaMakeId = 1;
    private const string HondaCode = "HONDA";
    private const string HondaName = "Honda";
    private const int CivicModelId = 1;
    private const string CivicCode = "CIVIC";
    private const string CivicName = "Civic";
    private const int NonExistentMakeId = 99;

    [Fact]
    public async Task GetByMakeAsync_MakeDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();
        var service = new ModelService(context);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => service.GetByMakeAsync(NonExistentMakeId));
    }

    [Fact]
    public async Task GetByMakeAsync_MakeHasNoModels_ReturnsEmptyList()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();
        context.Makes.Add(new Domain.Entities.Make { MakeId = HondaMakeId, Code = HondaCode, Name = HondaName });
        await context.SaveChangesAsync();
        var service = new ModelService(context);

        // Act
        var result = await service.GetByMakeAsync(HondaMakeId);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByMakeAsync_MultipleModels_ReturnsOrderedByName()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();
        var (make, _) = await DbSeeder.SeedMakeAndModelAsync(context);
        context.Models.Add(new Domain.Entities.Model { ModelId = 2, MakeId = make.MakeId, Code = "ACCORD", Name = "Accord" });
        await context.SaveChangesAsync();
        var service = new ModelService(context);

        // Act
        var result = await service.GetByMakeAsync(make.MakeId);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal(new[] { "Accord", CivicName }, result.Select(m => m.Name).ToArray());
    }

    [Fact]
    public async Task GetByMakeAsync_SingleModel_ReturnsCorrectResponseShape()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();
        var (make, _) = await DbSeeder.SeedMakeAndModelAsync(context);
        var service = new ModelService(context);

        // Act
        var result = await service.GetByMakeAsync(make.MakeId);

        // Assert
        var model = Assert.Single(result);
        Assert.Equal(CivicModelId, model.ModelId);
        Assert.Equal(HondaMakeId, model.MakeId);
        Assert.Equal(HondaName, model.MakeName);
        Assert.Equal(CivicCode, model.Code);
        Assert.Equal(CivicName, model.Name);
    }

    [Fact]
    public async Task GetByMakeAsync_MultipleMakes_ReturnsOnlyModelsForRequestedMake()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();
        context.Makes.AddRange(
            new Domain.Entities.Make { MakeId = 1, Code = HondaCode, Name = HondaName },
            new Domain.Entities.Make { MakeId = 2, Code = "TOYOTA", Name = "Toyota" }
        );
        context.Models.AddRange(
            new Domain.Entities.Model { ModelId = 1, MakeId = 1, Code = CivicCode, Name = CivicName },
            new Domain.Entities.Model { ModelId = 2, MakeId = 2, Code = "CAMRY", Name = "Camry" }
        );
        await context.SaveChangesAsync();
        var service = new ModelService(context);

        // Act
        var result = await service.GetByMakeAsync(1);

        // Assert
        var model = Assert.Single(result);
        Assert.Equal(CivicName, model.Name);
    }
}
