namespace SimpleShoppingApp.Data.Repository
{
    public interface IRepository<T> where T : class
    {
        Task AddAsync(T item);
        Task SaveChangesAsync();
        IQueryable<T> AllAsNoTracking();
    }
}
