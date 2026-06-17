namespace Domain.Dtos;

// MaintenanceId and PartId are provided via route
public record MaintenancePartRequest(
    int Quantity,
    decimal UnitPrice
);
