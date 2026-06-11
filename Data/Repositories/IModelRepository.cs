using Data.Models;

namespace Data.Repositories;

public interface IModelRepository : IRepository<Model, int>
{
    Task<Model?> FindByCodeAsync(string code, CancellationToken cancellationToken = default);
    IQueryable<Model> FindByMakeId(int makeId);
}
