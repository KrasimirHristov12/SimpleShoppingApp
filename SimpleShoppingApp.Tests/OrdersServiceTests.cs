using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using SimpleShoppingApp.Data;
using SimpleShoppingApp.Data.Models;
using SimpleShoppingApp.Data.Repository;
using SimpleShoppingApp.Services.Orders;
using SimpleShoppingApp.Models.Orders;
using SimpleShoppingApp.Data.Enums;
using SimpleShoppingApp.Services.Addresses;
using Moq;
using SimpleShoppingApp.Services.Products;
using SimpleShoppingApp.Services.Users;
using SimpleShoppingApp.Services.Emails;
using SimpleShoppingApp.Services.Notifications;
using SimpleShoppingApp.Services.Images;
using SimpleShoppingApp.Models.Images;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace SimpleShoppingApp.Tests
{
    public class OrdersServiceTests
    {
        private IRepository<Order> ordersRepository;
        private IRepository<Product> productsRepository;
        private IRepository<ShippingAddress> addressesRepo;
        private IOrdersService ordersService;
        private IAddressesService addressesService;
        private IProductsService productsService;
        private IUsersService usersService;
        private IEmailsService emailsService;
        private INotificationsService notificationsService;
        private IImagesService imagesService;
        private ApplicationDbContext db;

        [SetUp]
        public void Initialize()
        {
            var productsServiceMock = new Mock<IProductsService>();

            var usersServiceMock = new Mock<IUsersService>();

            var emailsServiceMock = new Mock<IEmailsService>();

            var notificationsServiceMock = new Mock<INotificationsService>();

            var imagesServiceMock = new Mock<IImagesService>();


            productsServiceMock.Setup(x => x.GetNameAsync(1))
                .ReturnsAsync("TestProdName");

            productsServiceMock.Setup(x => x.GetOwnerIdAsync(1))
                .ReturnsAsync("TestUserId");

            usersServiceMock.Setup(x => x.GetEmailAsync("TestUserId"))
                .ReturnsAsync("test@test.test");

            usersServiceMock.Setup(x => x.GetFullNameAsync("TestUserId"))
                .ReturnsAsync("Test Test");

            emailsServiceMock.Setup(x => x.SendAsync(string.Empty, string.Empty, string.Empty, string.Empty))
                .ReturnsAsync(true);

            notificationsServiceMock.Setup(x => x.AddAsync(string.Empty, string.Empty, string.Empty, null))
                .ReturnsAsync(true);

            imagesServiceMock.Setup(x => x.GetFirstAsync(1))
                .ReturnsAsync(new ImageViewModel());

            imagesServiceMock.Setup(x => x.GetFirstAsync(2))
                .ReturnsAsync(new ImageViewModel());

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            db = new ApplicationDbContext(options);
            ordersRepository = new Repository<Order>(db);
            productsRepository = new Repository<Product>(db);
            addressesRepo = new Repository<ShippingAddress>(db);
            addressesService = new AddressesService(addressesRepo);
            productsService = productsServiceMock.Object;
            usersService = usersServiceMock.Object;
            emailsService = emailsServiceMock.Object;
            notificationsService = notificationsServiceMock.Object;
            imagesService = imagesServiceMock.Object;
            ordersService = new OrdersService(ordersRepository, productsRepository, productsService, addressesService, emailsService, usersService, imagesService, notificationsService);
        }

        [Test]
        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(-10)]
        public async Task TestAddWhenAddressIdIsNotAPositiveNumber(int addressId)
        {
            var result = await ordersService.AddAsync(new MakeOrderInputModel
            {
                AddressId = addressId,
            }, string.Empty);

            Assert.That(result.Result, Is.EqualTo(MakeOrderResult.NotSpecifiedAddress));
        }



        [Test]
        public async Task TestAddWhenAddressDoesNotExist()
        {
            var result = await ordersService.AddAsync(new MakeOrderInputModel
            {
                AddressId = 2,
            }, string.Empty);

            Assert.That(result.Result, Is.EqualTo(MakeOrderResult.InvalidAddress));
        }

        [Test]
        public async Task TestAddWhenThereAreNoProducts()
        {
            await addressesRepo.AddAsync(new ShippingAddress
            {
                Name = "Test Address",
                UserId = "TestUserId"
            });

            await addressesRepo.SaveChangesAsync();

            var result = await ordersService.AddAsync(new MakeOrderInputModel
            {
                AddressId = 1,
                Quantities = new List<int> { 1, 2 },
            }, string.Empty);

            Assert.That(result.Result, Is.EqualTo(MakeOrderResult.SomethingWentWrong));
        }

        [Test]
        public async Task TestAddWhenThereAreNoQuantities()
        {
            await addressesRepo.AddAsync(new ShippingAddress
            {
                Name = "Test Address",
                UserId = "TestUserId"
            });

            await addressesRepo.SaveChangesAsync();

            var result = await ordersService.AddAsync(new MakeOrderInputModel
            {
                AddressId = 1,
                ProductIds = new List<int> { 1, 2 },
            }, string.Empty);

            Assert.That(result.Result, Is.EqualTo(MakeOrderResult.SomethingWentWrong));
        }

        [Test]
        public async Task TestAddWhenQuantitiesAndProductsCountsAreDifferent()
        {
            await addressesRepo.AddAsync(new ShippingAddress
            {
                Name = "Test Address",
                UserId = "TestUserId"
            });

            await addressesRepo.SaveChangesAsync();


            var result = await ordersService.AddAsync(new MakeOrderInputModel
            {
                AddressId = 1,
                ProductIds = new List<int> { 1, 2 },
                Quantities = new List<int> { 1 },
            }, string.Empty);

            Assert.That(result.Result, Is.EqualTo(MakeOrderResult.SomethingWentWrong));
        }


        [Test]
        [TestCase(0)]
        [TestCase(-1)]
        public async Task TestAddWhenThereIsInvalidQuantity(int quantity)
        {
            await addressesRepo.AddAsync(new ShippingAddress
            {
                Name = "Test Address",
                UserId = "TestUserId"
            });

            await addressesRepo.SaveChangesAsync();

            var result = await ordersService.AddAsync(new MakeOrderInputModel
            {
                AddressId = 1,
                ProductIds = new List<int> { 1 },
                Quantities = new List<int> { quantity },
            }, string.Empty);

            Assert.That(result.Result, Is.EqualTo(MakeOrderResult.InvalidQuantity));
        }

        [Test]
        [TestCase(0)]
        [TestCase(-1)]
        public async Task TestAddWhenProductIdIsInvalid(int productId)
        {
            await addressesRepo.AddAsync(new ShippingAddress
            {
                Name = "Test Address",
                UserId = "TestUserId"
            });

            await addressesRepo.SaveChangesAsync();

            var result = await ordersService.AddAsync(new MakeOrderInputModel
            {
                AddressId = 1,
                ProductIds = new List<int> { productId },
                Quantities = new List<int> { 1 },
            }, string.Empty);

            Assert.That(result.Result, Is.EqualTo(MakeOrderResult.SomethingWentWrong));
        }

        [Test]
        public async Task TestAddWhenProductDoesNotExist()
        {
            await addressesRepo.AddAsync(new ShippingAddress
            {
                Name = "Test Address",
                UserId = "TestUserId"
            });

            await addressesRepo.SaveChangesAsync();

            var result = await ordersService.AddAsync(new MakeOrderInputModel
            {
                AddressId = 1,
                ProductIds = new List<int> { 1 },
                Quantities = new List<int> { 1 },
            }, string.Empty);

            Assert.That(result.Result, Is.EqualTo(MakeOrderResult.SomethingWentWrong));
        }


        [Test]
        public async Task TestAddWhenProductQuantityIsLessThanThePassedQuantity()
        {
            await addressesRepo.AddAsync(new ShippingAddress
            {
                Name = "Test Address",
                UserId = "TestUserId"
            });

            await addressesRepo.SaveChangesAsync();

            await productsRepository.AddAsync(new Product
            {
                Name = "Test Prod",
                Description = "Test Description",
                Price = 30.50M,
                Quantity = 2,
                IsApproved = true,
                UserId = "TestUserId",
            });
            await productsRepository.SaveChangesAsync();
            var result = await ordersService.AddAsync(new MakeOrderInputModel
            {
                AddressId = 1,
                ProductIds = new List<int> { 1 },
                Quantities = new List<int> { 3 },
            }, "TestUserId");

            Assert.That(result.Result, Is.EqualTo(MakeOrderResult.InvalidQuantity));
        }

        [Test]
        public async Task TestAddOrderSuccessfully()
        {
            await addressesRepo.AddAsync(new ShippingAddress
            {
                Name = "Test Address",
                UserId = "TestUserId"
            });

            await addressesRepo.SaveChangesAsync();

            await productsRepository.AddAsync(new Product
            {
                Name = "Test Prod",
                Description = "Test Description",
                Price = 30.50M,
                Quantity = 3,
                IsApproved = true,
                UserId = "TestUserId",
            });
            await productsRepository.SaveChangesAsync();
            var result = await ordersService.AddAsync(new MakeOrderInputModel
            {
                AddressId = 1,
                ProductIds = new List<int> { 1 },
                Quantities = new List<int> { 3 },
                PhoneNumber = "0000000000",
            }, "TestUserId");

            Assert.That(result.Result, Is.EqualTo(MakeOrderResult.Success));
        }


        [Test]
        [TestCase(OrderStatus.Delivered)]
        [TestCase(OrderStatus.NotDelivered)]
        public async Task TestGetByStatus(OrderStatus orderStatus)
        {
            var dates = new List<DateTime> { DateTime.UtcNow, DateTime.UtcNow.AddDays(2), DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(-2), DateTime.UtcNow.AddDays(-40) };


            for (int i = 0; i < dates.Count; i++)
            {
                await ordersRepository.AddAsync(new Order
                {
                    DeliveryDate = dates[i],
                    UserId = "TestUserId",
                    PhoneNumber = "0000000000",

                });
            }

            await ordersRepository.SaveChangesAsync();

            var result = await ordersService.GetByStatusAsync(orderStatus, "TestUserId");
            if (orderStatus == OrderStatus.Delivered)
            {

                Assert.That(result.Count(), Is.EqualTo(4));
            }
            else if (orderStatus == OrderStatus.NotDelivered)
            {
                Assert.That(result.Count(), Is.EqualTo(2));
            }
        }

        [Test]
        public async Task TestGetOrderStatusWhenOrderDoesNotExist()
        {
            var result = await ordersService.GetOrderStatusAsync(1);
            Assert.IsNull(result);
        }

        [Test]
        public async Task TestGetOrderStatusWhenDelivered()
        {
            await ordersRepository.AddAsync(new Order
            {
                DeliveryDate = DateTime.UtcNow,
                UserId = "TestUserId",
                PhoneNumber = "0000000000",

            });

            await ordersRepository.SaveChangesAsync();
            var result = await ordersService.GetOrderStatusAsync(1);
            Assert.NotNull(result);
            Assert.That(result, Is.EqualTo(OrderStatus.Delivered));
        }

        [Test]
        public async Task TestGetOrderStatusWhenNotDelivered()
        {
            await ordersRepository.AddAsync(new Order
            {
                DeliveryDate = DateTime.UtcNow.AddDays(1),
                UserId = "TestUserId",
                PhoneNumber = "0000000000",

            });

            await ordersRepository.SaveChangesAsync();
            var result = await ordersService.GetOrderStatusAsync(1);
            Assert.NotNull(result);
            Assert.That(result, Is.EqualTo(OrderStatus.NotDelivered));
        }

        [Test]
        public async Task TestGetOrderDetailsWhenOrderDoesNotExist()
        {
            var result = await ordersService.GetOrderDetailsAsync(1);
            Assert.IsNull(result);
        }

        [Test]
        public async Task TestGetOrderDetailsWhenOrderExists()
        {
            await addressesRepo.AddAsync(new ShippingAddress
            {
                Name = "Test Address",
                UserId = "TestUserId"
            });

            await addressesRepo.SaveChangesAsync();

            await productsRepository.AddAsync(new Product
            {
                Name = "Test Prod",
                Description = "Test Description",
                Price = 30.50M,
                Quantity = 3,
                IsApproved = true,
                UserId = "TestUserId",
            });


            await productsRepository.AddAsync(new Product
            {
                Name = "Test Prod",
                Description = "Test Description",
                Price = 5,
                Quantity = 3,
                IsApproved = true,
                UserId = "TestUserId",
            });
            await productsRepository.SaveChangesAsync();

            await ordersService.AddAsync(new MakeOrderInputModel
            {
                AddressId = 1,
                ProductIds = new List<int> { 1,2 },
                Quantities = new List<int> { 3,3 },
                PhoneNumber = "0000000000",
                PaymentMethod = PaymentMethod.Card,
            }, "TestUserId");

            var result = await ordersService.GetOrderDetailsAsync(1);
            Assert.IsNotNull(result);
            var orderModel = result.Order;
            Assert.That(orderModel.PaymentMethod, Is.EqualTo("Credit / Debit Card"));
            Assert.That(orderModel.OrderStatus, Is.EqualTo("Not Delivered"));
            Assert.That(orderModel.TotalPrice, Is.EqualTo(106.5M));

        }

    }
}
