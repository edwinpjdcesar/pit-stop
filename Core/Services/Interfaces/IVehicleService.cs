using Domain.Dtos;

namespace Core.Services.Interfaces;

public interface IVehicleService
{
    Task<IReadOnlyList<VehicleResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<VehicleResponse?> GetByIdAsync(Guid vehicleId, CancellationToken cancellationToken = default);
    Task<VehicleResponse> CreateAsync(VehicleRequest request, CancellationToken cancellationToken = default);
    Task<VehicleResponse> UpdateAsync(Guid vehicleId, VehicleRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid vehicleId, CancellationToken cancellationToken = default);
}
