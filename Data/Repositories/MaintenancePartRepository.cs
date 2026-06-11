using Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data.Repositories;

public class MaintenancePartRepository : IMaintenancePartRepository
{
    private readonly PitStopContext _context;
    private readonly DbSet<MaintenancePart> _dbSet;
    private readonly ILogger<MaintenancePartRepository> _logger;

    public MaintenancePartRepository(PitStopContext context, ILogger<MaintenancePartRepository> logger)
    {
        _context = context;
        _dbSet = context.Set<MaintenancePart>();
        _logger = logger;
    }

    public async Task<MaintenancePart?> FindAsync(Guid maintenanceId, Guid partId, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Finding MaintenancePart for MaintenanceId {MaintenanceId} and PartId {PartId}", maintenanceId, partId);

        var entity = await _dbSet.FindAsync(new object?[] { maintenanceId, partId }, cancellationToken);

        if (entity is null)
            _logger.LogWarning("MaintenancePart not found for MaintenanceId {MaintenanceId} and PartId {PartId}", maintenanceId, partId);

        return entity;
    }

    public void Create(MaintenancePart entity)
    {
        _logger.LogDebug("Staging MaintenancePart for MaintenanceId {MaintenanceId} and PartId {PartId} for creation", entity.MaintenanceId, entity.PartId);
        _dbSet.Add(entity);
    }

    public void Update(MaintenancePart entity)
    {
        _logger.LogDebug("Staging MaintenancePart for MaintenanceId {MaintenanceId} and PartId {PartId} for update", entity.MaintenanceId, entity.PartId);
        _dbSet.Update(entity);
    }

    public void Delete(MaintenancePart entity)
    {
        _logger.LogDebug("Staging MaintenancePart for MaintenanceId {MaintenanceId} and PartId {PartId} for deletion", entity.MaintenanceId, entity.PartId);
        _dbSet.Remove(entity);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var affected = await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Saved {Count} change(s) for MaintenancePart", affected);
            return affected;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving changes for MaintenancePart");
            throw;
        }
    }
}
