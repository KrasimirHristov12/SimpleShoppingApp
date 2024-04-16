using SimpleShoppingApp.Models.Categories;

namespace SimpleShoppingApp.Services.Categories
{
    public interface ICategoriesService
    {

        Task<bool> AddAsync(string name);

        Task<bool> DoesCategoryExistAsync(int id);

        Task<IEnumerable<CategoryViewModel>> GetAllAsync();

        Task<IEnumerable<CategoryViewModel>> GetAllWithProductsAsync();

        Task<int> GetCountAsync();
    }
}
