using Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data.Repositories;

public abstract class Repository<T, U> : IRepository<T, U>
    where T : BaseEntity<U>
    where U : struct
{
    protected readonly PitStopContext Context;
    protected readonly DbSet<T> DbSet;
    protected readonly ILogger Logger;

    protected Repository(PitStopContext context, ILogger logger)
    {
        Context = context;
        DbSet = context.Set<T>();
        Logger = logger;
    }

    public async Task<T?> FindByIdAsync(U id, CancellationToken cancellationToken = default)
    {
        Logger.LogDebug("Finding {Entity} with id {Id}", typeof(T).Name, id);

        var entity = await DbSet.FindAsync(new object?[] { id }, cancellationToken);

        if (entity is null)
            Logger.LogWarning("{Entity} with id {Id} was not found", typeof(T).Name, id);

        return entity;
    }

    public IQueryable<T> FindAll()
    {
        Logger.LogDebug("Querying all {Entity} records", typeof(T).Name);
        return DbSet.AsQueryable();
    }

    public void Create(T entity)
    {
        Logger.LogDebug("Staging {Entity} for creation", typeof(T).Name);
        DbSet.Add(entity);
    }

    public void Update(T entity)
    {
        Logger.LogDebug("Staging {Entity} with id {Id} for update", typeof(T).Name, entity.Id);
        DbSet.Update(entity);
    }

    public void Delete(T entity)
    {
        Logger.LogDebug("Staging {Entity} with id {Id} for deletion", typeof(T).Name, entity.Id);
        DbSet.Remove(entity);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var affected = await Context.SaveChangesAsync(cancellationToken);
            Logger.LogInformation("Saved {Count} change(s) for {Entity}", affected, typeof(T).Name);
            return affected;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error saving changes for {Entity}", typeof(T).Name);
            throw;
        }
    }
}
