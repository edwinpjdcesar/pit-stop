using Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data.Repositories;

public class VehicleRepository : Repository<Vehicle, Guid>, IVehicleRepository
{
    public VehicleRepository(PitStopContext context, ILogger<VehicleRepository> logger)
        : base(context, logger) { }

    public async Task<Vehicle?> FindByVINAsync(string vin, CancellationToken cancellationToken = default)
    {
        Logger.LogDebug("Finding Vehicle with VIN {VIN}", vin);
        return await DbSet.FirstOrDefaultAsync(v => v.VIN == vin, cancellationToken);
    }

    public async Task<Vehicle?> FindByLicensePlateAsync(string licensePlate, CancellationToken cancellationToken = default)
    {
        Logger.LogDebug("Finding Vehicle with license plate {LicensePlate}", licensePlate);
        return await DbSet.FirstOrDefaultAsync(v => v.LicensePlate == licensePlate, cancellationToken);
    }
}
