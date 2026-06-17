using Domain.Dtos;

namespace Core.Services.Interfaces;

public interface IMaintenanceService
{
    Task<IReadOnlyList<MaintenanceResponse>> GetByVehicleAsync(Guid vehicleId, CancellationToken cancellationToken = default);
    Task<MaintenanceResponse> AddAsync(Guid vehicleId, MaintenanceRequest request, CancellationToken cancellationToken = default);
    Task<MaintenanceResponse> UpdateAsync(Guid vehicleId, Guid maintenanceId, MaintenanceRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid vehicleId, Guid maintenanceId, CancellationToken cancellationToken = default);
    Task ValidateOwnershipAsync(Guid vehicleId, Guid maintenanceId, CancellationToken cancellationToken = default);
}
