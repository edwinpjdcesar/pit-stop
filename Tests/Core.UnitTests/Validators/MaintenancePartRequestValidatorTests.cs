using Core.Validators;
using Domain.Dtos;
using FluentValidation.TestHelper;

namespace Core.UnitTests.Validators;

public class MaintenancePartRequestValidatorTests
{
    private readonly MaintenancePartRequestValidator _validator = new();

    private static MaintenancePartRequest ValidRequest() => new(Quantity: 2, UnitPrice: 12.99m);

    [Fact]
    public async Task Valid_request_passes_validation()
    {
        var result = await _validator.TestValidateAsync(ValidRequest());
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Quantity_zero_or_negative_fails(int quantity)
    {
        var result = await _validator.TestValidateAsync(ValidRequest() with { Quantity = quantity });
        result.ShouldHaveValidationErrorFor(x => x.Quantity);
    }

    [Fact]
    public async Task Quantity_one_passes()
    {
        var result = await _validator.TestValidateAsync(ValidRequest() with { Quantity = 1 });
        result.ShouldNotHaveValidationErrorFor(x => x.Quantity);
    }

    [Fact]
    public async Task Negative_UnitPrice_fails()
    {
        var result = await _validator.TestValidateAsync(ValidRequest() with { UnitPrice = -0.01m });
        result.ShouldHaveValidationErrorFor(x => x.UnitPrice);
    }

    [Fact]
    public async Task Zero_UnitPrice_passes()
    {
        var result = await _validator.TestValidateAsync(ValidRequest() with { UnitPrice = 0m });
        result.ShouldNotHaveValidationErrorFor(x => x.UnitPrice);
    }
}
