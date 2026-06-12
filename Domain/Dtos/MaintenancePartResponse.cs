namespace Domain.Dtos;

public record MaintenancePartResponse(
    Guid MaintenanceId,
    Guid PartId,
    string PartName,
    int Quantity,
    decimal UnitPrice
);
