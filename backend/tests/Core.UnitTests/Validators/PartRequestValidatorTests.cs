using Core.Validators;
using Domain.Dtos;
using FluentValidation.TestHelper;

namespace Core.UnitTests.Validators;

public class PartRequestValidatorTests
{
    private readonly PartRequestValidator _validator = new();

    private static PartRequest ValidRequest() => new(
        Name: "Oil Filter",
        ModelNumber: "OF-123",
        Description: "Standard oil filter"
    );

    [Fact]
    public async Task Valid_request_passes_validation()
    {
        var result = await _validator.TestValidateAsync(ValidRequest());
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Null_ModelNumber_passes()
    {
        var result = await _validator.TestValidateAsync(ValidRequest() with { ModelNumber = null });
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task Empty_or_null_name_fails(string? name)
    {
        var result = await _validator.TestValidateAsync(ValidRequest() with { Name = name! });
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task Empty_or_null_description_fails(string? description)
    {
        var result = await _validator.TestValidateAsync(ValidRequest() with { Description = description! });
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }
}
