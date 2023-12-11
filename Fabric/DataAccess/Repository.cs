namespace Fabric.DataAccess
{
    internal interface Repository<T>
    {
        Task<IEnumerable<T?>> GetAllAsync();
        Task CreateAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
    }
}
