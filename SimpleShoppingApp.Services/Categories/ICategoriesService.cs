using SimpleShoppingApp.Models.Categories;

namespace SimpleShoppingApp.Services.Categories
{
    public interface ICategoriesService
    {
        Task<bool> DoesCategoryExist(int id);
        Task<IEnumerable<CategoryViewModel>> GetAllAsync();

        Task<int> GetCountAsync();
    }
}
