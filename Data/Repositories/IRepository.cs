using Data.Models;

namespace Data.Repositories;

public interface IRepository<T, in U>
    where T : IEntity<U>
    where U : struct
{
    Task<T?> FindByIdAsync(U id, CancellationToken cancellationToken = default);
    IQueryable<T> FindAll();
    void Create(T entity);
    void Update(T entity);
    void Delete(T entity);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
