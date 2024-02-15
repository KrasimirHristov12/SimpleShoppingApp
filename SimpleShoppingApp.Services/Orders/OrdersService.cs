using SimpleShoppingApp.Data.Models;
using SimpleShoppingApp.Data.Repository;
using SimpleShoppingApp.Models.Orders;
using SimpleShoppingApp.Services.Products;

namespace SimpleShoppingApp.Services.Orders
{
    public class OrdersService : IOrdersService
    {
        private readonly IRepository<Order> orderRepo;
        private readonly IProductsService productsService;

        public OrdersService(IRepository<Order> _orderRepo, IProductsService _productsService)
        {
            orderRepo = _orderRepo;
            productsService = _productsService;
        }
        public async Task<bool> AddAsync(MakeOrderInputModel model, string userId)
        {
            var order = new Order
            {
                UserId = userId,
            };

            for (int i = 0; i < model.ProductIds.Count; i++)
            {
                int productId = model.ProductIds[i];
                int productQuantity = model.Quantities[i];

                if (productQuantity <= 0)
                {
                    return false;
                }

                if (productId <= 0 || !await productsService.DoesProductExistAsync(productId))
                {
                    return false;
                }

                order.OrdersProducts.Add(new OrdersProducts
                {
                    ProductId = productId,
                    Quantity = productQuantity,
                });
            }

            await orderRepo.AddAsync(order);
            await orderRepo.SaveChangesAsync();
            return true;

        }
    }
}
