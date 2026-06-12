namespace Domain.Dtos;

public record VehicleResponse(
    Guid VehicleId,
    string? VIN,
    string? LicensePlate,
    int Year,
    MakeResponse Make,
    ModelResponse Model,
    DateOnly? PurchaseDate,
    decimal? PurchasePrice,
    int? MileageAtPurchase,
    int? Mileage
);

