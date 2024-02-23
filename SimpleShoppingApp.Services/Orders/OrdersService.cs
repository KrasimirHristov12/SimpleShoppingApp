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
        private readonly IRepository<Product> productRepo;
        private readonly IProductsService productsService;
        private readonly IAddressesService addressesService;

        public OrdersService(IRepository<Order> _orderRepo,
            IRepository<Product> _productRepo,
            IProductsService _productsService,
            IAddressesService _addressesService)
        {
            orderRepo = _orderRepo;
            productRepo = _productRepo;
            productsService = _productsService;
            addressesService = _addressesService;
        }
        public async Task<MakeOrderResult> AddAsync(MakeOrderInputModel model, string userId)
        {
            if (model.AddressId <= 0)
            {
                return MakeOrderResult.NotFound;
            }

            if (!await addressesService.DoesAddressExistAsync(model.AddressId))
            {
                return MakeOrderResult.NotFound;
            }

            if (model.ProductIds.Count == 0 || model.Quantities.Count == 0)
            {
                return MakeOrderResult.NotFound;
            }

            if (model.ProductIds.Count != model.Quantities.Count)
            {
                return MakeOrderResult.NotFound;
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

                if (productQuantity <= 0)
                {
                    return MakeOrderResult.InvalidQuantity;
                }

                if (productId <= 0)
                {
                    return MakeOrderResult.NotFound;
                }

                var actualProduct = await productRepo.AllAsTracking()
                    .FirstOrDefaultAsync(p => p.Id == productId);

                if (actualProduct == null)
                {
                    return MakeOrderResult.NotFound;
                }

                var actualProductQuantity = actualProduct.Quantity;



                if (productQuantity > actualProductQuantity)
                {
                    return MakeOrderResult.InvalidQuantity;
                }

                order.OrdersProducts.Add(new OrdersProducts
                {
                    ProductId = productId,
                    Quantity = productQuantity,
                });

                actualProduct.Quantity = actualProductQuantity - productQuantity;

            }

            await orderRepo.AddAsync(order);
            await orderRepo.SaveChangesAsync();
            return MakeOrderResult.Success;

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
