namespace ChatUp.Data.Repositories;

public interface IRepository<T> where T : class
{
    T? Get(int id);
    T? Get(Expression<Func<T, bool>> predicate);
    bool Exists(Expression<Func<T, bool>> predicate);
    IEnumerable<T> GetList(Expression<Func<T, bool>> predicate);
    IEnumerable<T> GetList();
    int Count();
    int Save();

    Task<T?> GetAsync(int id);
    Task<T?> GetAsync(Expression<Func<T, bool>> predicate);
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
    Task<IEnumerable<T>> GetListAsync(Expression<Func<T, bool>> predicate);
    Task<IEnumerable<T>> GetListAsync();
    Task<int> CountAsync();
    Task<int> SaveAsync();

    void Insert(T objModel);
    void Insert(IEnumerable<T> objModel);
    void Update(T objModel);
    void Remove(T objModel);
    void Dispose();

    Task InsertAsync(T objModel);
    Task InsertAsync(IEnumerable<T> objModel);
}