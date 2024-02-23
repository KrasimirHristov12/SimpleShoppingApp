using SimpleShoppingApp.Data.Enums;
using SimpleShoppingApp.Models.Products;

namespace SimpleShoppingApp.Services.Products
{
    public interface IProductsService
    {
        Task<AddProductModel> AddAsync(AddProductInputModel model, string userId);

        Task<ProductViewModel?> GetAsync(int id, string? userId);

        Task<IEnumerable<ListProductsViewModel>> GetRandomProductsAsync(int n);

        Task<IEnumerable<ListProductsViewModel>?> GetByCategoryAsync(int categoryId, int elementsPerPage, int currentPage);

        Task<AddUpdateDeleteResult> DeleteAsync(int id, string currentUserId);

        Task<int> GetCountForCategoryAsync(int categoryId);

        Task<EditProductModel> GetToEditAsync(int id, string currentUserId);

        Task<AddUpdateDeleteResult> UpdateAsync(EditProductInputModel model, string currentUserId);

        Task<IEnumerable<ListProductsViewModel>> GetByNameAsync(string name);

        Task<bool> BelognsToUserAsync(int productId, string loggedInUserId);

        Task<int?> GetQuantityAsync(int id);

        Task<bool> DoesProductExistAsync(int productId);
    }
}
