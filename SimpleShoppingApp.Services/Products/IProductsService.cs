using SimpleShoppingApp.Data.Enums;
using SimpleShoppingApp.Models.Products;

namespace SimpleShoppingApp.Services.Products
{
    public interface IProductsService
    {
        Task<AddProductModel> AddAsync(AddProductInputModel model, string userId, string imagesDirectory);

        Task<ProductViewModel?> GetAsync(int id, string? userId);

        Task<IEnumerable<ListProductsViewModel>> GetRandomProductsAsync(int n);

        Task<ProductsPerPageModel?> GetByCategoryAsync(ProductsFilterModel model);

        Task<AddUpdateDeleteResult> DeleteAsync(int id, string currentUserId);

        Task<int> GetCountForCategoryAsync(int categoryId);

        Task<EditProductModel> GetToEditAsync(int id, string currentUserId);

        Task<ApproveProductViewModel?> GetProductToApproveAsync(int productId);

        Task<AddUpdateDeleteResult> UpdateAsync(EditProductInputModel model, string currentUserId);

        Task<IEnumerable<ListProductsViewModel>> GetByNameAsync(string name);

        Task<bool> BelognsToUserAsync(int productId, string loggedInUserId);

        Task<int?> GetQuantityAsync(int id);

        Task<bool> DoesProductExistAsync(int productId);

        Task<ProductRatingViewModel> AddRatingFromUserAsync(int productId, string loggedInUserId, int rating);

        Task<int> GetCountAsync();
    }
}
