using Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data.Repositories;

public class ModelRepository : Repository<Model, int>, IModelRepository
{
    public ModelRepository(PitStopContext context, ILogger<ModelRepository> logger)
        : base(context, logger) { }

    public async Task<Model?> FindByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        Logger.LogDebug("Finding Model with code {Code}", code);
        return await DbSet.FirstOrDefaultAsync(m => m.Code == code, cancellationToken);
    }

    public IQueryable<Model> FindByMakeId(int makeId)
    {
        Logger.LogDebug("Querying Models for MakeId {MakeId}", makeId);
        return DbSet.Where(m => m.MakeId == makeId);
    }
}
