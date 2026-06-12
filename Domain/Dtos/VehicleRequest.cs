namespace Domain.Dtos;

public record VehicleRequest(
    int MakeId,
    int ModelId,
    string? VIN,
    string? LicensePlate,
    int Year,
    DateOnly? PurchaseDate,
    decimal? PurchasePrice,
    int? MileageAtPurchase,
    int? Mileage
);

