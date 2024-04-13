using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using SimpleShoppingApp.Data;
using SimpleShoppingApp.Data.Enums;
using SimpleShoppingApp.Data.Models;
using SimpleShoppingApp.Data.Repository;
using SimpleShoppingApp.Services.Carts;
using SimpleShoppingApp.Services.Products;
using SimpleShoppingApp.Services.Users;

namespace SimpleShoppingApp.Tests
{
    public class CartsServiceTests
    {
        private ApplicationDbContext db;
        private IRepository<ShoppingCart> cartsRepo;
        private IRepository<CartsProducts> cartsProductsRepo;
        private ICartsService cartsService;
        private IProductsService productsService;
        private IUsersService usersService;

        [SetUp]
        public void Initialize()
        {
            var productsServiceMock = new Mock<IProductsService>();

            var usersServiceMock = new Mock<IUsersService>();

            productsServiceMock.Setup(x => x.DoesProductExistAsync(1))
                .ReturnsAsync(false);

            productsServiceMock.Setup(x => x.DoesProductExistAsync(2))
                .ReturnsAsync(true);

            productsServiceMock.Setup(x => x.DoesProductExistAsync(3))
                .ReturnsAsync(true);

            productsServiceMock.Setup(x => x.DoesProductExistAsync(4))
                .ReturnsAsync(true);

            productsServiceMock.Setup(x => x.GetQuantityAsync(2))
                .ReturnsAsync((int?)null);

            productsServiceMock.Setup(x => x.GetQuantityAsync(3))
                .ReturnsAsync(0);

            productsServiceMock.Setup(x => x.GetQuantityAsync(4))
                .ReturnsAsync(1);

            usersServiceMock.Setup(x => x.GetAdminIdAsync())
                .ReturnsAsync("TestAdminId");


            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

            db = new ApplicationDbContext(options);
            cartsRepo = new Repository<ShoppingCart>(db);
            cartsProductsRepo = new Repository<CartsProducts>(db);
            productsService = productsServiceMock.Object;
            usersService = usersServiceMock.Object;
            cartsService = new CartsService(cartsRepo, cartsProductsRepo, productsService, null, usersService);
        }

        [Test]
        public async Task TestAddCart()
        {
            var cartId = await cartsService.AddAsync("TestUserId");
            Assert.That(cartId, Is.EqualTo(1));
        }

        [Test]
        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(-10)]
        public async Task TestCartExistsWhenCartIdIsNotAPositiveNumber(int cartId)
        {
            var result = await cartsService.CartExistsAsync(cartId);
            Assert.IsFalse(result);
        }

        [Test]
        public async Task TestCartExistsWhenCartDoesNotExists()
        {
            await cartsService.AddAsync("TestUserId");
            var result = await cartsService.CartExistsAsync(2);
            Assert.IsFalse(result);
        }

        [Test]
        public async Task TestCartExistsWhenCartExists()
        {
            var cartId = await cartsService.AddAsync("TestUserId");
            var result = await cartsService.CartExistsAsync(cartId);
            Assert.IsTrue(result);
        }

        [Test]
        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(-10)]
        public async Task TestAddProductWhenCartIdIsNotAPositiveNumber(int cartId)
        {
            var result = await cartsService.AddProductAsync(cartId, 1, string.Empty);
            Assert.That(result, Is.EqualTo(AddUpdateProductToCartResult.NotFound));
        }

        [Test]
        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(-10)]
        public async Task TestAddProductWhenProductIdIsNotAPositiveNumber(int productId)
        {
            var result = await cartsService.AddProductAsync(1, productId, string.Empty);
            Assert.That(result, Is.EqualTo(AddUpdateProductToCartResult.NotFound));
        }

        [Test]
        public async Task TestAddProductWhenProductDoesNotExist()
        {
            var result = await cartsService.AddProductAsync(1, 1, string.Empty);
            Assert.That(result, Is.EqualTo(AddUpdateProductToCartResult.NotFound));
        }

        [Test]
        public async Task TestAddProductWhenCartDoesNotExist()
        {
            var result = await cartsService.AddProductAsync(1, 2, string.Empty);
            Assert.That(result, Is.EqualTo(AddUpdateProductToCartResult.NotFound));
        }

        [Test]
        public async Task TestAddProductWhenTriedByNotOwner()
        {
            await cartsService.AddAsync("TestUserId");
            var result = await cartsService.AddProductAsync(1, 2, "TestUserId2");
            Assert.That(result, Is.EqualTo(AddUpdateProductToCartResult.Forbidden));
        }

        [Test]
        public async Task TestAddProductWhenProductExistsInCart()
        {
            await cartsService.AddAsync("TestUserId");
            await cartsProductsRepo.AddAsync(new CartsProducts
            {
                CartId = 1,
                ProductId = 2,
            });
            await cartsProductsRepo.SaveChangesAsync();
            var result = await cartsService.AddProductAsync(1, 2, "TestUserId");
            Assert.That(result, Is.EqualTo(AddUpdateProductToCartResult.AlreadyExist));
        }

        [Test]
        public async Task TestAddProductWhenProductQuantityIsNull()
        {
            await cartsService.AddAsync("TestUserId");
            await cartsProductsRepo.AddAsync(new CartsProducts
            {
                CartId = 1,
                ProductId = 2,
                IsDeleted = true,
            });
            await cartsProductsRepo.SaveChangesAsync();

            var result = await cartsService.AddProductAsync(1, 2, "TestUserId");
            Assert.That(result, Is.EqualTo(AddUpdateProductToCartResult.NotFound));
        }

        [Test]
        public async Task TestAddProductWhenProductQuantityIsZero()
        {
            await cartsService.AddAsync("TestUserId");
            await cartsProductsRepo.AddAsync(new CartsProducts
            {
                CartId = 1,
                ProductId = 3,
                IsDeleted = true,
            });
            await cartsProductsRepo.SaveChangesAsync();

            var result = await cartsService.AddProductAsync(1, 3, "TestUserId");
            Assert.That(result, Is.EqualTo(AddUpdateProductToCartResult.NotInStock));
        }


        [Test]
        public async Task TestAddProductSuccessfullyWhenProductExistsInCart()
        {
            await cartsService.AddAsync("TestUserId");
            await cartsProductsRepo.AddAsync(new CartsProducts
            {
                CartId = 1,
                ProductId = 4,
                IsDeleted = true,
            });
            await cartsProductsRepo.SaveChangesAsync();

            var result = await cartsService.AddProductAsync(1, 4, "TestUserId");
            Assert.That(result, Is.EqualTo(AddUpdateProductToCartResult.Success));
        }


        [Test]
        public async Task TestAddProductSuccessfullyWhenProductDoesNotExistInCart()
        {
            await cartsService.AddAsync("TestUserId");
            var result = await cartsService.AddProductAsync(1, 4, "TestUserId");
            Assert.That(result, Is.EqualTo(AddUpdateProductToCartResult.Success));
        }

        [Test]
        public async Task TestGetCartWhenUserDoesNotHaveOne()
        {
            var result = await cartsService.GetAsync("TestUserId");
            Assert.IsNull(result);
        }

        [Test]
        public async Task TestGetCartSuccessfully()
        {
            await cartsService.AddAsync("TestUserId");
            var result = await cartsService.GetAsync("TestUserId");
            Assert.IsNotNull(result);
        }

        [Test]
        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(-10)]
        public async Task TestRemoveProductFromCartWhenCartIdIsNotAPositiveNumber(int cartId)
        {

        }


    }
}
