using Data.Models;

namespace Data.Repositories;

public interface IPartRepository : IRepository<Part, Guid>
{
    Task<Part?> FindByNameAsync(string name, CancellationToken cancellationToken = default);
}
