using Core.Validators;
using Domain.Dtos;
using FluentValidation.TestHelper;

namespace Core.UnitTests.Validators;

public class MaintenanceRequestValidatorTests
{
    private readonly MaintenanceRequestValidator _validator = new();

    private static MaintenanceRequest ValidRequest() => new(
        Description: "Oil change",
        Mileage: 24000,
        ServiceDate: new DateTime(2026, 5, 1, 10, 30, 0, DateTimeKind.Utc)
    );

    [Fact]
    public async Task Valid_request_passes_validation()
    {
        var result = await _validator.TestValidateAsync(ValidRequest());
        result.ShouldNotHaveAnyValidationErrors();
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

    [Fact]
    public async Task Negative_mileage_fails()
    {
        var result = await _validator.TestValidateAsync(ValidRequest() with { Mileage = -1 });
        result.ShouldHaveValidationErrorFor(x => x.Mileage);
    }

    [Fact]
    public async Task Zero_mileage_passes()
    {
        var result = await _validator.TestValidateAsync(ValidRequest() with { Mileage = 0 });
        result.ShouldNotHaveValidationErrorFor(x => x.Mileage);
    }

    [Fact]
    public async Task Default_ServiceDate_fails()
    {
        var result = await _validator.TestValidateAsync(ValidRequest() with { ServiceDate = default });
        result.ShouldHaveValidationErrorFor(x => x.ServiceDate);
    }
}
