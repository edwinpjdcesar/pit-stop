namespace Domain.Dtos;

// VehicleId is provided via route, not request body
public record MaintenanceRequest(
    string Description,
    int Mileage,
    DateTime ServiceDate
);
