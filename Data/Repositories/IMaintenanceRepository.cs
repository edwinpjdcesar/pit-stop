using Data.Models;

namespace Data.Repositories;

public interface IMaintenanceRepository : IRepository<Maintenance, Guid>
{
    IQueryable<Maintenance> FindByVehicleId(Guid vehicleId);
}
