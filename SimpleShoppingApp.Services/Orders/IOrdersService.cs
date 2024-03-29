using SimpleShoppingApp.Data.Enums;
using SimpleShoppingApp.Models.Orders;

namespace SimpleShoppingApp.Services.Orders
{
    public interface IOrdersService
    {
        Task<MakeOrderResultModel> AddAsync(MakeOrderInputModel model, string userId);

        Task<IEnumerable<OrderViewModel>> GetByStatusAsync(OrderStatus status, string userId);

        Task<OrderDetailsViewModel?> GetOrderDetailsAsync(int orderId);

        Task<OrderStatus?> GetOrderStatusAsync(int orderId);

    }
}
