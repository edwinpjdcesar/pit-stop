using Core.IntegrationTests.Helpers;
using Core.Services;
using Domain.Dtos;
using Domain.Exceptions;

namespace Core.IntegrationTests.Services;

public class PartServiceTests
{
    private const string OilFilterName = "Oil Filter";
    private const string OilFilterDescription = "Standard oil filter";
    private const string UpdatedName = "Premium Oil Filter";
    private const string UpdatedModelNumber = "OF-999";
    private const string UpdatedDescription = "Synthetic grade filter";

    [Fact]
    public async Task GetAllAsync_EmptyDatabase_ReturnsEmptyList()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();
        var service = new PartService(context, NullLogger<PartService>.Instance);

        // Act
        var result = await service.GetAllAsync();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_MultipleParts_ReturnsOrderedByName()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();
        await DbSeeder.SeedPartAsync(context, name: "Wiper Blades", description: "Replacement wipers");
        await DbSeeder.SeedPartAsync(context, name: "Air Filter", description: "Engine air filter");
        await DbSeeder.SeedPartAsync(context, name: OilFilterName, description: OilFilterDescription);
        var service = new PartService(context, NullLogger<PartService>.Instance);

        // Act
        var result = await service.GetAllAsync();

        // Assert
        Assert.Equal(new[] { "Air Filter", OilFilterName, "Wiper Blades" }, result.Select(p => p.Name).ToArray());
    }

    [Fact]
    public async Task GetByIdAsync_PartDoesNotExist_ReturnsNull()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();
        var service = new PartService(context, NullLogger<PartService>.Instance);

        // Act
        var result = await service.GetByIdAsync(Guid.NewGuid());

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_PartExists_ReturnsCorrectPart()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();
        var part = await DbSeeder.SeedPartAsync(context);
        var service = new PartService(context, NullLogger<PartService>.Instance);

        // Act
        var result = await service.GetByIdAsync(part.PartId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(part.PartId, result.PartId);
        Assert.Equal(OilFilterName, result.Name);
    }

    [Fact]
    public async Task CreateAsync_ValidRequest_ReturnsCreatedPart()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();
        var service = new PartService(context, NullLogger<PartService>.Instance);
        var request = new PartRequest(Name: "Brake Pads", ModelNumber: "BP-456", Description: "Front brake pads");

        // Act
        var result = await service.CreateAsync(request);

        // Assert
        Assert.NotEqual(Guid.Empty, result.PartId);
        Assert.Equal("Brake Pads", result.Name);
        Assert.Equal("BP-456", result.ModelNumber);
        Assert.Equal("Front brake pads", result.Description);
    }

    [Fact]
    public async Task CreateAsync_ValidRequest_PersistsPartToDatabase()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();
        var service = new PartService(context, NullLogger<PartService>.Instance);
        var request = new PartRequest(Name: OilFilterName, ModelNumber: null, Description: OilFilterDescription);

        // Act
        await service.CreateAsync(request);

        // Assert
        Assert.Single(context.Parts);
    }

    [Fact]
    public async Task CreateAsync_NullModelNumber_ReturnsPartWithNullModelNumber()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();
        var service = new PartService(context, NullLogger<PartService>.Instance);
        var request = new PartRequest(Name: OilFilterName, ModelNumber: null, Description: OilFilterDescription);

        // Act
        var result = await service.CreateAsync(request);

        // Assert
        Assert.Null(result.ModelNumber);
    }

    [Fact]
    public async Task UpdateAsync_PartDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();
        var service = new PartService(context, NullLogger<PartService>.Instance);
        var request = new PartRequest(Name: UpdatedName, ModelNumber: null, Description: UpdatedDescription);

        // Act
        var ex = await Assert.ThrowsAsync<NotFoundException>(() => service.UpdateAsync(Guid.NewGuid(), request));

        // Assert
        Assert.Contains("Part", ex.Message);
    }

    [Fact]
    public async Task UpdateAsync_ValidRequest_ReturnsUpdatedPart()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();
        var part = await DbSeeder.SeedPartAsync(context);
        var service = new PartService(context, NullLogger<PartService>.Instance);
        var request = new PartRequest(Name: UpdatedName, ModelNumber: UpdatedModelNumber, Description: UpdatedDescription);

        // Act
        var result = await service.UpdateAsync(part.PartId, request);

        // Assert
        Assert.Equal(UpdatedName, result.Name);
        Assert.Equal(UpdatedModelNumber, result.ModelNumber);
        Assert.Equal(UpdatedDescription, result.Description);
    }

    [Fact]
    public async Task DeleteAsync_PartDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();
        var service = new PartService(context, NullLogger<PartService>.Instance);

        // Act
        var ex = await Assert.ThrowsAsync<NotFoundException>(() => service.DeleteAsync(Guid.NewGuid()));

        // Assert
        Assert.Contains("Part", ex.Message);
    }

    [Fact]
    public async Task DeleteAsync_PartExists_RemovesPartFromDatabase()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();
        var part = await DbSeeder.SeedPartAsync(context);
        var service = new PartService(context, NullLogger<PartService>.Instance);

        // Act
        await service.DeleteAsync(part.PartId);

        // Assert
        Assert.Empty(context.Parts);
    }
}
