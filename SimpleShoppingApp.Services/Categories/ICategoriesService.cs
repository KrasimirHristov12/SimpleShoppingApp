using SimpleShoppingApp.Models.Categories;

namespace SimpleShoppingApp.Services.Categories
{
    public interface ICategoriesService
    {
        Task<IEnumerable<CategoryViewModel>> GetAllAsync();
    }
}
