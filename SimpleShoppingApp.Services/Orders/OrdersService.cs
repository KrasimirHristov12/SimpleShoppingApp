using Microsoft.EntityFrameworkCore;
using SimpleShoppingApp.Data.Enums;
using SimpleShoppingApp.Data.Models;
using SimpleShoppingApp.Data.Repository;
using SimpleShoppingApp.Models.Orders;
using SimpleShoppingApp.Services.Addresses;
using SimpleShoppingApp.Services.Emails;
using SimpleShoppingApp.Services.Products;
using SimpleShoppingApp.Services.Users;

namespace SimpleShoppingApp.Services.Orders
{
    public class OrdersService : IOrdersService
    {
        private readonly IRepository<Order> orderRepo;
        private readonly IRepository<Product> productRepo;
        private readonly IRepository<OrdersProducts> ordersProductsRepo;
        private readonly IProductsService productsService;
        private readonly IAddressesService addressesService;
        private readonly IEmailsService emailsService;
        private readonly IUsersService usersService;

        public OrdersService(IRepository<Order> _orderRepo,
            IRepository<Product> _productRepo,
            IRepository<OrdersProducts> _ordersProductsRepo,
            IProductsService _productsService,
            IAddressesService _addressesService,
            IEmailsService _emailsService,
            IUsersService _usersService)
        {
            orderRepo = _orderRepo;
            productRepo = _productRepo;
            ordersProductsRepo = _ordersProductsRepo;
            productsService = _productsService;
            addressesService = _addressesService;
            emailsService = _emailsService;
            usersService = _usersService;
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

            var email = await usersService.GetEmailAsync(userId);
            var fullName = await usersService.GetFullNameAsync(userId);
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(fullName))
            {
                return MakeOrderResult.NotFound;
            }

            var productsInfo = await ordersProductsRepo
                .AllAsNoTracking()
                .Where(op => op.OrderId == order.Id)
                .Select(op => new OrderProductViewModel
                {
                    Name = op.Product.Name,
                    Address = op.Order.Address.Name,
                    Quantity = op.Quantity,
                    Price = op.Product.Price,
                    DeliveryDays = (op.Order.DeliveryDate - op.Order.CreatedOn).Days,
                    
                })
            .ToListAsync();

            decimal orderTotalPrice = productsInfo.Sum(p => p.TotalPrice);
            string tableHtml = "<table><tr><th style=\"border: 1px solid #ddd;\">Name</th><th style=\"border: 1px solid #ddd;\">Quantity</th><th style=\"border: 1px solid #ddd;\">Price</th></tr>";
            foreach (var prod in productsInfo)
            {
                tableHtml += $"<tr><td style=\"border: 1px solid #ddd;\">{prod.Name}</td><td style=\"border: 1px solid #ddd;\">{prod.Quantity}</td><td style=\"border: 1px solid #ddd;\">${prod.Price}</td></tr>";
            }
            tableHtml += "</table>";
            string priceHtml = $"<div><b>Total Price</b>: ${orderTotalPrice:F2}</div>";
            string addressHtml = $"<div><b>Address</b>: {productsInfo.First().Address}</div><br/><br/>";
            string deliveryHtml = $"<hr/><br/><div><b>Delivery Days</b>: {productsInfo.First().DeliveryDays} days from now.</div>";
            string emailSubject = $"Order #{order.Id}";
            string emailContent = $"Hi {fullName},<br/><br/>Thanks for the order!<br/><br/><b>Order Details</b>:<br/><br/>{addressHtml}{tableHtml}{deliveryHtml}{priceHtml}<br/><br/>Best regards,<br/>SimpleShoppingApp Team";
            var emailResult = await emailsService.SendAsync(email, fullName, emailSubject, emailContent);
            return MakeOrderResult.Success;

        }

        public async Task<IEnumerable<OrderViewModel>> GetByStatusAsync(OrderStatus status, string userId)
        {
            var query = orderRepo.AllAsNoTracking();

            if (status == OrderStatus.Delivered)
            {
               query = query.Where(o => o.DeliveryDate <= DateTime.UtcNow);
            }
            else
            {
                query = query.Where(o => o.DeliveryDate > DateTime.UtcNow);

            }

            return await query.Where(o => o.UserId == userId && !o.IsDeleted)
                 .Select(o => new OrderViewModel
                 {
                     Id = o.Id,
                     TotalPrice = o.OrdersProducts.Where(op => !op.IsDeleted).Select(op => op.Product.Price * op.Quantity).Sum(),

                 }).ToListAsync();
        }
    }
}
