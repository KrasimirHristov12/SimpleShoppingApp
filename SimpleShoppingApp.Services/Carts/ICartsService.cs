using SimpleShoppingApp.Data.Enums;
using SimpleShoppingApp.Models.Carts;
using System.Threading.Tasks;

namespace SimpleShoppingApp.Services.Carts
{
    public interface ICartsService
    {
        Task<int> AddAsync(string userId);

        Task<bool> CartExistsAsync(int id);

        Task<AddUpdateProductToCartResult> AddProductAsync(int cartId, int productId, string currentUserId);

        Task<UpdateQuantityModel> UpdateQuantityInCartAsync(int cartId, int productId, int updatedQuantity, string currentUserId);

        Task<CartViewModel?> GetAsync(string currentUserId);

        Task<int> GetIdAsync(string userId);

        Task<RemoveProductFromCartModel> RemoveProductAsync(int cartId, int productId, string currentUserId);

        Task<AddUpdateDeleteResult> RemoveAllProductsAsync(int cartId, string currentUserId);

        Task<string?> GetUserIdAsync(int id);

        Task<bool> BelongsToUserAsync(string ownerUserId, string currentUserId);

    }
}
