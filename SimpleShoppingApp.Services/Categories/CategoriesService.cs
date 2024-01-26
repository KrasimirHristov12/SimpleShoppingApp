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
        public async Task<IEnumerable<CategoryViewModel>> GetAllAsync()
        {
            var categories = await categoryRepo.AllAsNoTracking()
                .Select(c => new CategoryViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                }).ToListAsync();

            return categories;
        }

        public async Task<int> GetCountOfProductsAsync(int categoryId)
        {
            return await categoryRepo.AllAsNoTracking()
                .Where(c => c.Id == categoryId)
                .Select(c => c.Products.Count)
                .FirstOrDefaultAsync();
        }
    }
}
