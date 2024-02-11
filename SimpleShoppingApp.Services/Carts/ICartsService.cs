using SimpleShoppingApp.Data.Enums;
using SimpleShoppingApp.Models.Carts;

namespace SimpleShoppingApp.Services.Carts
{
    public interface ICartsService
    {
        Task AddAsync(string userId);

        Task<bool> DoesUserHaveCartAsync(string userId);

        Task<bool> AddProductAsync(int cartId, int productId);

        Task<int?> UpdateQuantityInCartAsync(int cartId, int productId, QuantityOperation operation);

        Task<CartViewModel?> GetAsync(string userId);

        Task<int> GetIdAsync(string userId);
    }
}
