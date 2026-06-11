using Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data.Repositories;

public class PartRepository : Repository<Part, Guid>, IPartRepository
{
    public PartRepository(PitStopContext context, ILogger<PartRepository> logger)
        : base(context, logger) { }

    public async Task<Part?> FindByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        Logger.LogDebug("Finding Part with name {Name}", name);
        return await DbSet.FirstOrDefaultAsync(p => p.Name == name, cancellationToken);
    }
}
