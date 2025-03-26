using ChatUp.Data.Database;

namespace ChatUp.Data.Repositories;

public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
{
    protected readonly AppDbContext _context;

    public Repository(AppDbContext context)
    {
        _context = context;
    }

    public TEntity? Get(int id)
    {
        return _context.Set<TEntity>().Find(id);
    }

    public TEntity? Get(Expression<Func<TEntity, bool>> predicate)
    {
        return _context.Set<TEntity>().FirstOrDefault(predicate);
    }
    public bool Exists(Expression<Func<TEntity, bool>> predicate)
    {
        return _context.Set<TEntity>().Any(predicate);
    }

    public IEnumerable<TEntity> GetList(Expression<Func<TEntity, bool>> predicate)
    {
        return _context.Set<TEntity>().Where(predicate).ToList();
    }

    public IEnumerable<TEntity> GetList()
    {
        return _context.Set<TEntity>().ToList();
    }

    public int Count()
    {
        return _context.Set<TEntity>().Count();
    }
    public int Save()
    {
        return _context.SaveChanges();
    }



    public async Task<TEntity?> GetAsync(int id)
    {
        return await _context.Set<TEntity>().FindAsync(id);
    }

    public async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _context.Set<TEntity>().FirstOrDefaultAsync(predicate);
    }

    public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _context.Set<TEntity>().AnyAsync(predicate);
    }

    public async Task<IEnumerable<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _context.Set<TEntity>().Where(predicate).ToListAsync();
    }

    public async Task<IEnumerable<TEntity>> GetListAsync()
    {
        return await _context.Set<TEntity>().ToListAsync();
    }

    public async Task<int> CountAsync()
    {
        return await _context.Set<TEntity>().CountAsync();
    }

    public Task<int> SaveAsync()
    {
        return _context.SaveChangesAsync();
    }



    public void Insert(TEntity objModel)
    {
        _context.Set<TEntity>().Add(objModel);
    }

    public void Insert(IEnumerable<TEntity> objModel)
    {
        _context.Set<TEntity>().AddRange(objModel);
    }

    public void Update(TEntity objModel)
    {
        _context.Entry(objModel).State = EntityState.Modified;
    }

    public void Remove(TEntity objModel)
    {
        _context.Set<TEntity>().Remove(objModel);
    }



    public async Task InsertAsync(TEntity objModel)
    {
        await _context.Set<TEntity>().AddAsync(objModel);
    }

    public async Task InsertAsync(IEnumerable<TEntity> objModel)
    {
        await _context.Set<TEntity>().AddRangeAsync(objModel);
    }



    public void Dispose()
    {
        _context.Dispose();
    }
}
