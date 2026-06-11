using Data.Models;

namespace Data.Repositories;

public interface IMaintenancePartRepository
{
    Task<MaintenancePart?> FindAsync(Guid maintenanceId, Guid partId, CancellationToken cancellationToken = default);
    void Create(MaintenancePart entity);
    void Update(MaintenancePart entity);
    void Delete(MaintenancePart entity);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
