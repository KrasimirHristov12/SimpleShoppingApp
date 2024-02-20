using SimpleShoppingApp.Data.Enums;
using SimpleShoppingApp.Models.Carts;

namespace SimpleShoppingApp.Services.Carts
{
    public interface ICartsService
    {
        Task<int> AddAsync(string userId);

        Task<bool> DoesUserHaveCartAsync(string userId);

        Task AddProductAsync(int cartId, int productId);

        Task<int?> UpdateQuantityInCartAsync(int cartId, int productId, QuantityOperation operation);

        Task<CartViewModel?> GetAsync(string userId);

        Task<int?> GetIdAsync(string userId);

        Task<CartJsonViewModel?> RemoveProductAsync(int cartId, int productId);

        Task RemoveAllProductsAsync(int cartId);

        Task<UpdateQuantityJsonViewModel?> UpdateProductQuantityAsync(int cartId, int productId, int updatedQuantity);

    }
}
