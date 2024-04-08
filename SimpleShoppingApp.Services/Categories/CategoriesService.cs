using Microsoft.EntityFrameworkCore;
using SimpleShoppingApp.Data.Models;
using SimpleShoppingApp.Data.Repository;
using SimpleShoppingApp.Models.Categories;

namespace SimpleShoppingApp.Services.Categories
{
    public class CategoriesService : ICategoriesService
    {
        private readonly IRepository<Category> categoryRepo;

        public CategoriesService(IRepository<Category> _categoryRepo)
        {
            categoryRepo = _categoryRepo;
        }

        public async Task<bool> AddAsync(string name)
        {
            if (categoryRepo.AllAsNoTracking().Any(c => !c.IsDeleted && c.Name == name))
            {
                return false;
            }

            var category = new Category
            {
                Name = name,
            };

            await categoryRepo.AddAsync(category);

            await categoryRepo.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DoesCategoryExistAsync(int id)
        {
            if (id <= 0)
            {
                return false;
            }

            return await categoryRepo
                .AllAsNoTracking()
                .AnyAsync(c => c.Id == id && !c.IsDeleted);
        }
        public async Task<IEnumerable<CategoryViewModel>> GetAllAsync()
        {
            var categories = await categoryRepo.AllAsNoTracking()
                .Where(c => !c.IsDeleted)
                .Select(c => new CategoryViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                }).ToListAsync();

            return categories;
        }

        public async Task<int> GetCountAsync()
        {
            return await categoryRepo.AllAsNoTracking()
                .Where(c => !c.IsDeleted)
                .CountAsync();
        }
    }
}
