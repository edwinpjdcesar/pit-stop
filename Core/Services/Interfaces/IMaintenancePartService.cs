using Domain.Dtos;

namespace Core.Services.Interfaces;

public interface IMaintenancePartService
{
    Task<MaintenancePartResponse> AddPartAsync(Guid maintenanceId, Guid partId, MaintenancePartRequest request, CancellationToken cancellationToken = default);
    Task<MaintenancePartResponse> UpdatePartAsync(Guid maintenanceId, Guid partId, MaintenancePartRequest request, CancellationToken cancellationToken = default);
    Task RemovePartAsync(Guid maintenanceId, Guid partId, CancellationToken cancellationToken = default);
}
