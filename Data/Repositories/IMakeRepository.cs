using Data.Models;

namespace Data.Repositories;

public interface IMakeRepository : IRepository<Make, int>
{
    Task<Make?> FindByCodeAsync(string code, CancellationToken cancellationToken = default);
}
