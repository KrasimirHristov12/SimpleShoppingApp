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

        public async Task<bool> DoesCategoryExist(int id)
        {
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

    }
}
