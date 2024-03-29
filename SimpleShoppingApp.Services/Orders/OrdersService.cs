using Microsoft.EntityFrameworkCore;
using SimpleShoppingApp.Data.Enums;
using SimpleShoppingApp.Data.Models;
using SimpleShoppingApp.Data.Repository;
using SimpleShoppingApp.Extensions;
using SimpleShoppingApp.Models.Orders;
using SimpleShoppingApp.Models.Images;
using SimpleShoppingApp.Services.Addresses;
using SimpleShoppingApp.Services.Emails;
using SimpleShoppingApp.Services.Images;
using SimpleShoppingApp.Services.Notifications;
using SimpleShoppingApp.Services.Products;
using SimpleShoppingApp.Services.Users;

namespace SimpleShoppingApp.Services.Orders
{
    public class OrdersService : IOrdersService
    {
        private readonly IRepository<Order> orderRepo;
        private readonly IRepository<Product> productRepo;
        private readonly IProductsService productsService;
        private readonly IAddressesService addressesService;
        private readonly IEmailsService emailsService;
        private readonly IUsersService usersService;
        private readonly IImagesService imagesService;
        private readonly INotificationsService notificationsService;

        public OrdersService(IRepository<Order> _orderRepo,
            IRepository<Product> _productRepo,
            IProductsService _productsService,
            IAddressesService _addressesService,
            IEmailsService _emailsService,
            IUsersService _usersService,
            IImagesService _imagesService,
            INotificationsService _notificationsService)
        {
            orderRepo = _orderRepo;
            productRepo = _productRepo;
            productsService = _productsService;
            addressesService = _addressesService;
            emailsService = _emailsService;
            usersService = _usersService;
            imagesService = _imagesService;
            notificationsService = _notificationsService;
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
                PaymentMethod = model.PaymentMethod,
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

            var orderInfo = await GetOrderDetailsAsync(order.Id);
            if (orderInfo == null)
            {
                return MakeOrderResult.NotFound;
            }

            string address = orderInfo.Order.Address;
            int deliveryDays = orderInfo.Order.DeliveryDays;
            var paymentMethod = orderInfo.Order.PaymentMethod;
            decimal orderTotalPrice = orderInfo.Order.TotalPrice;
            string tableHtml = "<table><tr><th style=\"border: 1px solid #ddd;\"></th><th style=\"border: 1px solid #ddd;\">Name</th><th style=\"border: 1px solid #ddd;\">Quantity</th><th style=\"border: 1px solid #ddd;\">Price</th></tr>";
            foreach (var prod in orderInfo.Products)
            {
                tableHtml += $"<tr><td style=\"border: 1px solid #ddd;\"><img width=\"100px\" src=\"https://localhost:7287/images/products/{prod.Image.Name}{prod.Image.Extension}\"/></td><td style=\"border: 1px solid #ddd;\">{prod.Name}</td><td style=\"border: 1px solid #ddd;\">{prod.Quantity}</td><td style=\"border: 1px solid #ddd;\">${prod.Price}</td></tr>";
            }
            tableHtml += "</table><hr/><br/>";
            string priceHtml = $"<div><b>Total Price</b>: ${orderTotalPrice:F2}</div>";
            string addressHtml = $"<div><b>Address</b>: {address}</div>";
            string paymentHtml = $"<div><b>Payment Method</b>: {paymentMethod}</div>";
            string deliveryHtml = $"<div><b>Delivery Days</b>: {deliveryDays} days from now.</div>";
            string emailSubject = $"Order #{order.Id}";
            string emailContent = $"Hi {fullName},<br/><br/>Thanks for the order!<br/><br/><b>Order Details</b>:<br/><br/>{tableHtml}{addressHtml}{deliveryHtml}{paymentHtml}{priceHtml}<br/><br/>Best regards,<br/>SimpleShoppingApp Team";
            var emailResult = await emailsService.SendAsync(email, fullName, emailSubject, emailContent);

            for (int i = 0; i < model.ProductIds.Count; i++)
            {
                var ownerId = await productsService.GetOwnerIdAsync(model.ProductIds[i]);
                if (ownerId == null)
                {
                    return MakeOrderResult.NotFound;
                }

                var buyerEmail = await usersService.GetEmailAsync(userId);

                if (buyerEmail == null)
                {
                    return MakeOrderResult.NotFound;
                }

                var notificationResult = await notificationsService.AddAsync(userId, ownerId, $"{buyerEmail} has just bought {model.Quantities[i]} pieces of one of your products", $"/Products/Index/{model.ProductIds[i]}");

            }


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

        public async Task<OrderStatus?> GetOrderStatusAsync(int orderId)
        {
            var order = await orderRepo
                .AllAsNoTracking()
                .Where(o => o.Id == orderId && !o.IsDeleted)
                .Select(o => new
                {
                    DeliveryDate = o.DeliveryDate,
                })
               .FirstOrDefaultAsync();
            if (order == null)
            {
                return null;
            }
            if (DateTime.UtcNow >= order.DeliveryDate)
            {
                return OrderStatus.Delivered;
            }
            return OrderStatus.NotDelivered;
        }

        public async Task<OrderDetailsViewModel?> GetOrderDetailsAsync(int orderId)
        {
            var orderProduct = await orderRepo.AllAsNoTracking()
                .Where(o => o.Id == orderId && !o.IsDeleted)
                .Select(o => new OrderDetailsViewModel
                {
                    Order = new OrderOrderDetailsViewModel { 
                        Id = o.Id,
                        Address = o.Address.Name,
                        CreatedOn = o.CreatedOn,
                        DeliveryDate = o.DeliveryDate,
                        DeliveryDays = (o.DeliveryDate - o.CreatedOn).Days,
                        PaymentMethodEnum = o.PaymentMethod,
                        
                    },
                    Products = o.OrdersProducts.Select(op => new OrderProductDetailsViewModel
                    {
                        Id = op.ProductId,
                        Name = op.Product.Name,
                        Price = op.Product.Price,
                        Quantity = op.Quantity,
                        Image = op.Product.Images.Select(i => new ImageViewModel
                        {
                            Extension = i.Extension,
                            Name = i.Name,
                            ImageUrl = i.ImageUrl,

                        }).First()
                    }).ToList(),
                })
                .FirstOrDefaultAsync();

            if (orderProduct == null)
            {
                return null;
            }

            OrderStatus? orderStatusEnum = await GetOrderStatusAsync(orderProduct.Order.Id);
            if (orderStatusEnum == null)
            {
                return null;
            }
            orderProduct.Order.PaymentMethod = orderProduct.Order.PaymentMethodEnum.GetDisplayName();

            orderProduct.Order.OrderStatus = orderStatusEnum.GetDisplayName();

            orderProduct.Order.TotalPrice = orderProduct.Products.Sum(p => p.Price * p.Quantity);

            foreach (var prod in orderProduct.Products)
            {
                var productImage = await imagesService.GetFirstAsync(prod.Id);
                if (productImage == null)
                {
                    return null;
                }
                prod.Image = productImage;   
            }

            return orderProduct;

        }
    }
}
