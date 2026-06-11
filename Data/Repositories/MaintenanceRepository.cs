using Data.Models;
using Microsoft.Extensions.Logging;

namespace Data.Repositories;

public class MaintenanceRepository : Repository<Maintenance, Guid>, IMaintenanceRepository
{
    public MaintenanceRepository(PitStopContext context, ILogger<MaintenanceRepository> logger)
        : base(context, logger) { }

    public IQueryable<Maintenance> FindByVehicleId(Guid vehicleId)
    {
        Logger.LogDebug("Querying Maintenance records for VehicleId {VehicleId}", vehicleId);
        return DbSet.Where(m => m.VehicleId == vehicleId);
    }
}
