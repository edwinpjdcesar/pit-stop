using Data.Models;

namespace Data.Repositories;

public interface IVehicleRepository : IRepository<Vehicle, Guid>
{
    Task<Vehicle?> FindByVINAsync(string vin, CancellationToken cancellationToken = default);
    Task<Vehicle?> FindByLicensePlateAsync(string licensePlate, CancellationToken cancellationToken = default);
}
