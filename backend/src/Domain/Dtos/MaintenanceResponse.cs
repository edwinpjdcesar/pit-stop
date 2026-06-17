namespace Domain.Dtos;

public record MaintenanceResponse(
    Guid MaintenanceId,
    Guid VehicleId,
    string Description,
    int Mileage,
    DateTime ServiceDate,
    IReadOnlyList<MaintenancePartResponse> MaintenanceParts
);
