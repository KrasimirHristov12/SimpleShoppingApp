using Microsoft.EntityFrameworkCore;
using SimpleShoppingApp.Data.Enums;
using SimpleShoppingApp.Data.Models;
using SimpleShoppingApp.Data.Repository;
using SimpleShoppingApp.Models.Orders;
using SimpleShoppingApp.Services.Addresses;
using SimpleShoppingApp.Services.Products;

namespace SimpleShoppingApp.Services.Orders
{
    public class OrdersService : IOrdersService
    {
        private readonly IRepository<Order> orderRepo;
        private readonly IProductsService productsService;
        private readonly IAddressesService addressesService;

        public OrdersService(IRepository<Order> _orderRepo, 
            IProductsService _productsService,
            IAddressesService _addressesService)
        {
            orderRepo = _orderRepo;
            productsService = _productsService;
            addressesService = _addressesService;
        }
        public async Task<AddUpdateDeleteResult> AddAsync(MakeOrderInputModel model, string userId)
        {
            if (model.AddressId <= 0)
            {
                return AddUpdateDeleteResult.NotFound;
            }

            if (!await addressesService.DoesAddressExistAsync(model.AddressId))
            {
                return AddUpdateDeleteResult.NotFound;
            }

            if (model.ProductIds.Count == 0 || model.Quantities.Count == 0)
            {
                return AddUpdateDeleteResult.NotFound;
            }

            if (model.ProductIds.Count != model.Quantities.Count)
            {
                return AddUpdateDeleteResult.NotFound;
            }

            var order = new Order
            {
                UserId = userId,
                PaymentMethod = model.PaymentMethod,
                PhoneNumber = model.PhoneNumber,
                AddressId = model.AddressId,
            };

            for (int i = 0; i < model.ProductIds.Count; i++)
            {
                int productId = model.ProductIds[i];
                int productQuantity = model.Quantities[i];

                if (productQuantity <= 0 || productId <= 0)
                {
                    return AddUpdateDeleteResult.NotFound;
                }

                if (!await productsService.DoesProductExistAsync(productId))
                {
                    return AddUpdateDeleteResult.NotFound;
                }

                order.OrdersProducts.Add(new OrdersProducts
                {
                    ProductId = productId,
                    Quantity = productQuantity,
                });
            }

            await orderRepo.AddAsync(order);
            await orderRepo.SaveChangesAsync();
            return AddUpdateDeleteResult.Success;

        }

        public async Task<IEnumerable<OrderViewModel>> GetByStatusAsync(OrderStatus status, string userId)
        {
           return await orderRepo.AllAsNoTracking()
                .Where(o => o.OrderStatus == status && o.UserId == userId && !o.IsDeleted)
                .Select(o => new OrderViewModel
                {
                    Id = o.Id,
                    TotalPrice = o.OrdersProducts.Where(op => !op.IsDeleted).Select(op => op.Product.Price * op.Quantity).Sum(),

                }).ToListAsync();
        }
    }
}
