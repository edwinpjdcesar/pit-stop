using Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data.Repositories;

public class MakeRepository : Repository<Make, int>, IMakeRepository
{
    public MakeRepository(PitStopContext context, ILogger<MakeRepository> logger)
        : base(context, logger) { }

    public async Task<Make?> FindByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        Logger.LogDebug("Finding Make with code {Code}", code);
        return await DbSet.FirstOrDefaultAsync(m => m.Code == code, cancellationToken);
    }
}
