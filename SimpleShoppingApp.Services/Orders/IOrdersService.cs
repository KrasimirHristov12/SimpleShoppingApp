using SimpleShoppingApp.Models.Orders;

namespace SimpleShoppingApp.Services.Orders
{
    public interface IOrdersService
    {
        Task<bool> AddAsync(MakeOrderInputModel model, string userId);
    }
}
