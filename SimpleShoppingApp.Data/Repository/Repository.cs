using Microsoft.EntityFrameworkCore;

namespace SimpleShoppingApp.Data.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext db;

        public Repository(ApplicationDbContext _db)
        {
            db = _db;
        }

        public IQueryable<T> AllAsNoTracking()
        {
            return db.Set<T>().AsNoTracking();
        }

        public IQueryable<T> AllAsTracking()
        {
            return db.Set<T>().AsTracking();
        }

        public async Task AddAsync(T item)
        {
            await db.AddAsync<T>(item);
        }

        public async Task SaveChangesAsync()
        {
            await db.SaveChangesAsync();
        }

    }
}
