﻿using SimpleShoppingApp.Data.Enums;
using SimpleShoppingApp.Models.Products;

namespace SimpleShoppingApp.Services.Products
{
    public interface IProductsService
    {
        Task<AddProductModel> AddAsync(AddProductInputModel model, string userId, string imagesDirectory);

        Task<ProductViewModel?> GetAsync(int id, string? userId);

        Task<string?> GetNameAsync(int productId);

        Task<IEnumerable<ListProductsViewModel>> GetRandomProductsAsync(int n);

        Task<ProductsPerPageModel?> GetByCategoryAsync(ProductsFilterModel model);

        Task<AddUpdateDeleteResult> DeleteAsync(int id, string currentUserId);

        Task<int> GetCountForCategoryAsync(int categoryId);

        Task<EditProductModel> GetToEditAsync(int id, string currentUserId);

        Task<AddUpdateDeleteResult> UpdateAsync(EditProductInputModel model, string currentUserId);

        Task<IEnumerable<ListProductsViewModel>> GetByNameAsync(string name);

        Task<bool> BelognsToUserAsync(int productId, string loggedInUserId);

        Task<int?> GetQuantityAsync(int id);

        Task<bool> DoesProductExistAsync(int productId);

        Task<ProductRatingViewModel> AddRatingFromUserAsync(int productId, string loggedInUserId, int rating, string? reviewText);

        Task<string?> GetReviewTextAsync(string userId, int productId);

        Task<IEnumerable<ProductReviewViewModel>> GetReviewsAsync(int productId, string? userName);

        Task<int> GetCountAsync();

        Task<string?> GetOwnerIdAsync(int productId);

        Task<bool> ApproveAsync(int productId, string userId);

        Task<bool> UnApproveAsync(int productId, string userId);
    }
}
