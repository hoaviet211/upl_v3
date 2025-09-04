using System.Linq.Expressions;
using Academy.Crm.Core.Entities;
using Academy.Crm.Core.Interfaces;
using Academy.Crm.Infra.Db;
using Microsoft.EntityFrameworkCore;

namespace Academy.Crm.Infra.Repositories;

public class Repository<T> : IRepository<T> where T : BaseEntity
{
    private readonly AppDbContext _db;
    private readonly DbSet<T> _set;

    public Repository(AppDbContext db)
    {
        _db = db;
        _set = _db.Set<T>();
    }

    public IQueryable<T> Query() => _set.AsQueryable();

    public Task<T?> GetByIdAsync(int id, CancellationToken ct = default)
        => _set.FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task AddAsync(T entity, CancellationToken ct = default)
        => await _set.AddAsync(entity, ct);

    public async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken ct = default)
        => await _set.AddRangeAsync(entities, ct);

    public void Update(T entity) => _set.Update(entity);

    public void Remove(T entity)
    {
        // Soft delete if supported
        if (entity.GetType().GetProperty("IsDeleted") != null)
        {
            entity.GetType().GetProperty("IsDeleted")!.SetValue(entity, true);
            _set.Update(entity);
        }
        else
        {
            _set.Remove(entity);
        }
    }

    public Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
        => _set.AnyAsync(predicate, ct);
}

