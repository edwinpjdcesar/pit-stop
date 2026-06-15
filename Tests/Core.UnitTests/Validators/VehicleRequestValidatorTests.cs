using Core.Validators;
using Domain.Dtos;
using FluentValidation.TestHelper;

namespace Core.UnitTests.Validators;

public class VehicleRequestValidatorTests
{
    private readonly VehicleRequestValidator _validator = new();

    private static VehicleRequest ValidRequest() => new(
        MakeId: 1,
        ModelId: 10,
        VIN: null,
        LicensePlate: null,
        Year: 2020,
        PurchaseDate: null,
        PurchasePrice: null,
        MileageAtPurchase: null,
        Mileage: null
    );

    [Fact]
    public async Task Valid_request_passes_validation()
    {
        var result = await _validator.TestValidateAsync(ValidRequest());
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task MakeId_zero_or_negative_fails(int makeId)
    {
        var result = await _validator.TestValidateAsync(ValidRequest() with { MakeId = makeId });
        result.ShouldHaveValidationErrorFor(x => x.MakeId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public async Task ModelId_zero_or_negative_fails(int modelId)
    {
        var result = await _validator.TestValidateAsync(ValidRequest() with { ModelId = modelId });
        result.ShouldHaveValidationErrorFor(x => x.ModelId);
    }

    [Theory]
    [InlineData(1885)]
    [InlineData(0)]
    [InlineData(-100)]
    public async Task Year_below_1886_fails(int year)
    {
        var result = await _validator.TestValidateAsync(ValidRequest() with { Year = year });
        result.ShouldHaveValidationErrorFor(x => x.Year);
    }

    [Fact]
    public async Task Year_in_valid_range_passes()
    {
        var result = await _validator.TestValidateAsync(ValidRequest() with { Year = 1886 });
        result.ShouldNotHaveValidationErrorFor(x => x.Year);
    }

    [Fact]
    public async Task Negative_PurchasePrice_fails()
    {
        var result = await _validator.TestValidateAsync(ValidRequest() with { PurchasePrice = -1m });
        result.ShouldHaveValidationErrorFor(x => x.PurchasePrice);
    }

    [Fact]
    public async Task Zero_PurchasePrice_passes()
    {
        var result = await _validator.TestValidateAsync(ValidRequest() with { PurchasePrice = 0m });
        result.ShouldNotHaveValidationErrorFor(x => x.PurchasePrice);
    }

    [Fact]
    public async Task Null_PurchasePrice_passes()
    {
        var result = await _validator.TestValidateAsync(ValidRequest() with { PurchasePrice = null });
        result.ShouldNotHaveValidationErrorFor(x => x.PurchasePrice);
    }

    [Fact]
    public async Task Negative_MileageAtPurchase_fails()
    {
        var result = await _validator.TestValidateAsync(ValidRequest() with { MileageAtPurchase = -1 });
        result.ShouldHaveValidationErrorFor(x => x.MileageAtPurchase);
    }

    [Fact]
    public async Task Negative_Mileage_fails()
    {
        var result = await _validator.TestValidateAsync(ValidRequest() with { Mileage = -1 });
        result.ShouldHaveValidationErrorFor(x => x.Mileage);
    }

    [Fact]
    public async Task All_optional_fields_populated_passes()
    {
        var request = new VehicleRequest(
            MakeId: 1,
            ModelId: 10,
            VIN: "1HGCM82633A123456",
            LicensePlate: "ABC123",
            Year: 2020,
            PurchaseDate: new DateOnly(2023, 1, 15),
            PurchasePrice: 22000m,
            MileageAtPurchase: 15000,
            Mileage: 24500
        );

        var result = await _validator.TestValidateAsync(request);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
