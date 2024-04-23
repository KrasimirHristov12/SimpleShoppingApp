using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using SimpleShoppingApp.Data;
using SimpleShoppingApp.Data.Enums;
using SimpleShoppingApp.Data.Models;
using SimpleShoppingApp.Data.Repository;
using SimpleShoppingApp.Models.Products;
using SimpleShoppingApp.Services.Carts;
using SimpleShoppingApp.Services.Categories;
using SimpleShoppingApp.Services.Products;
using SimpleShoppingApp.Services.Users;

namespace SimpleShoppingApp.Tests
{
    public class CartsServiceTests
    {
        private ApplicationDbContext db;
        private IRepository<ShoppingCart> cartsRepo;
        private IRepository<CartsProducts> cartsProductsRepo;
        private IRepository<Product> productsRepo;
        private IRepository<UsersRating> usersRatingRepo;
        private ICartsService cartsService;
        private IProductsService productsService;
        private IUsersService usersService;
        private ICategoriesService categoriesService;

        [SetUp]
        public void Initialize()
        {
            var usersServiceMock = new Mock<IUsersService>();

            var categoriesServiceMock = new Mock<ICategoriesService>();

            usersServiceMock.Setup(x => x.GetAdminIdAsync())
                .ReturnsAsync("TestAdminId");

            usersServiceMock.Setup(x => x.IsInRoleAsync("TestUserId", "Administrator"))
                .ReturnsAsync(false);

            categoriesServiceMock.Setup(x => x.DoesCategoryExistAsync(1))
                .ReturnsAsync(true);


            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

            db = new ApplicationDbContext(options);
            cartsRepo = new Repository<ShoppingCart>(db);
            productsRepo = new Repository<Product>(db);
            cartsProductsRepo = new Repository<CartsProducts>(db);
            usersRatingRepo = new Repository<UsersRating>(db);
            usersService = usersServiceMock.Object;
            categoriesService = categoriesServiceMock.Object;
            productsService = new ProductsService(productsRepo, usersRatingRepo, cartsProductsRepo, null, categoriesService, usersService, null, null);
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
            var result = await cartsService.AddProductAsync(1, 2, string.Empty);
            Assert.That(result, Is.EqualTo(AddUpdateProductToCartResult.NotFound));
        }

        [Test]
        public async Task TestAddProductWhenCartDoesNotExist()
        {
            var result = await cartsService.AddProductAsync(1, 1, string.Empty);
            Assert.That(result, Is.EqualTo(AddUpdateProductToCartResult.NotFound));
        }

        [Test]
        public async Task TestAddProductWhenTriedByNotOwner()
        {
            await productsService.AddAsync(new AddProductInputModel
            {
                CategoryId = 1,
                Name = "TestProd",
                Price = 2M,
                Quantity = 2,
                Description = "Test Description",

            }, "TestUserId", string.Empty);
            await productsService.ApproveAsync(1, "TestAdminId");

            await cartsService.AddAsync("TestUserId");
            var result = await cartsService.AddProductAsync(1, 1, "TestUserId2");
            Assert.That(result, Is.EqualTo(AddUpdateProductToCartResult.Forbidden));
        }

        [Test]
        public async Task TestAddProductWhenTriedByOwnerOfProduct()
        {
            await productsService.AddAsync(new AddProductInputModel
            {
                CategoryId = 1,
                Name = "TestProd",
                Price = 2M,
                Quantity = 2,
                Description = "Test Description",

            }, "TestUserId", string.Empty);
            await productsService.ApproveAsync(1, "TestAdminId");

            await cartsService.AddAsync("TestUserId");
            var result = await cartsService.AddProductAsync(1, 1, "TestUserId");
            Assert.That(result, Is.EqualTo(AddUpdateProductToCartResult.Forbidden));
        }

        [Test]
        public async Task TestAddProductWhenProductExistsInCart()
        {
            await productsService.AddAsync(new AddProductInputModel
            {
                CategoryId = 1,
                Name = "TestProd",
                Price = 2M,
                Quantity = 1,
                Description = "Test Description",
            }, "TestUserId", string.Empty);

            await productsService.ApproveAsync(1, "TestAdminId");

            await cartsService.AddAsync("TestUserId2");
            await cartsService.AddProductAsync(1, 1, "TestUserId2");
            var result = await cartsService.AddProductAsync(1, 1, "TestUserId2");
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
            await productsService.AddAsync(new AddProductInputModel
            {
                CategoryId = 1,
                Name = "TestProd",
                Price = 2M,
                Quantity = 0,
                Description = "Test Description",
            }, "TestUserId", string.Empty);

            await productsService.ApproveAsync(1, "TestAdminId");

            await cartsService.AddAsync("TestUserId2");

            await cartsProductsRepo.AddAsync(new CartsProducts
            {
                CartId = 1,
                ProductId = 1,
                IsDeleted = true,
            });
            await cartsProductsRepo.SaveChangesAsync();

            var result = await cartsService.AddProductAsync(1, 1, "TestUserId2");

            Assert.That(result, Is.EqualTo(AddUpdateProductToCartResult.NotInStock));
        }


        [Test]
        public async Task TestAddProductSuccessfullyWhenProductExistsInCart()
        {
            await productsService.AddAsync(new AddProductInputModel
            {
                CategoryId = 1,
                Name = "TestProd",
                Price = 2M,
                Quantity = 1,
                Description = "Test Description",
            }, "TestUserId", string.Empty);

            await productsService.ApproveAsync(1, "TestAdminId");

            await cartsService.AddAsync("TestUserId2");
            await cartsProductsRepo.AddAsync(new CartsProducts
            {
                CartId = 1,
                ProductId = 1,
                IsDeleted = true,
            });
            await cartsProductsRepo.SaveChangesAsync();

            var result = await cartsService.AddProductAsync(1, 1, "TestUserId2");
            Assert.That(result, Is.EqualTo(AddUpdateProductToCartResult.Success));
        }


        [Test]
        public async Task TestAddProductSuccessfullyWhenProductDoesNotExistInCart()
        {
            await productsService.AddAsync(new AddProductInputModel
            {
                CategoryId = 1,
                Name = "TestProd",
                Price = 2M,
                Quantity = 1,
                Description = "Test Description",
            }, "TestUserId", string.Empty);

            await productsService.ApproveAsync(1, "TestAdminId");

            await cartsService.AddAsync("TestUserId2");
            var result = await cartsService.AddProductAsync(1, 1, "TestUserId2");
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
            var result = await cartsService.RemoveProductAsync(cartId, 1, string.Empty);
            Assert.That(result.Result, Is.EqualTo(AddUpdateDeleteResult.NotFound));
        }

        [Test]
        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(-10)]
        public async Task TestRemoveProductFromCartWhenProductIdIsNotAPositiveNumber(int productId)
        {
            var result = await cartsService.RemoveProductAsync(1, productId, string.Empty);
            Assert.That(result.Result, Is.EqualTo(AddUpdateDeleteResult.NotFound));
        }

        [Test]
        public async Task TestRemoveProductFromCartProductDoesNotExistInTheCart()
        {
            var result = await cartsService.RemoveProductAsync(1, 1, string.Empty);
            Assert.That(result.Result, Is.EqualTo(AddUpdateDeleteResult.NotFound));
        }


        [Test]
        public async Task TestRemoveProductFromCartWhenCartDoesNotExist()
        {
            await productsService.AddAsync(new AddProductInputModel
            {
                CategoryId = 1,
                Name = "TestProd",
                Price = 2M,
                Quantity = 1,
                Description = "Test Description",
            }, "TestUserId", string.Empty);

            await productsService.ApproveAsync(1, "TestAdminId");

            await cartsProductsRepo.AddAsync(new CartsProducts
            {
                CartId = 1,
                ProductId = 1,
            });
            await cartsProductsRepo.SaveChangesAsync();
            var result = await cartsService.RemoveProductAsync(1, 1, string.Empty);
            Assert.That(result.Result, Is.EqualTo(AddUpdateDeleteResult.NotFound));
        }

        [Test]
        public async Task TestRemoveProductFromCartWhenTriedByNotOwner()
        {
            await productsService.AddAsync(new AddProductInputModel
            {
                CategoryId = 1,
                Name = "TestProd",
                Price = 2M,
                Quantity = 1,
                Description = "Test Description",
            }, "TestUserId", string.Empty);

            await productsService.ApproveAsync(1, "TestAdminId");

            await cartsService.AddAsync("TestUserId");

            await cartsProductsRepo.AddAsync(new CartsProducts
            {
                CartId = 1,
                ProductId = 1,
            });
            await cartsProductsRepo.SaveChangesAsync();
            var result = await cartsService.RemoveProductAsync(1, 1, "TestUserId2");
            Assert.That(result.Result, Is.EqualTo(AddUpdateDeleteResult.Forbidden));
        }


        [Test]
        public async Task TestRemoveProductFromCartSuccessfullyWhenThereIsOneProduct()
        {
            await productsService.AddAsync(new AddProductInputModel
            {
                CategoryId = 1,
                Name = "TestProd",
                Price = 2M,
                Quantity = 1,
                Description = "Test Description",
            }, "TestUserId", string.Empty);

            await productsService.ApproveAsync(1, "TestAdminId");

            await cartsService.AddAsync("TestUserId");

            await cartsProductsRepo.AddAsync(new CartsProducts
            {
                CartId = 1,
                ProductId = 1,
            });
            await cartsProductsRepo.SaveChangesAsync();
            var result = await cartsService.RemoveProductAsync(1, 1, "TestUserId");
            var model = result.Model;
            Assert.That(result.Result, Is.EqualTo(AddUpdateDeleteResult.Success));
            Assert.NotNull(model);
            Assert.That(model.ProductId, Is.EqualTo(1));
            Assert.That(model.NewCount, Is.EqualTo(0));
            Assert.That(model.NewTotalPrice, Is.EqualTo(0));

        }


        [Test]
        public async Task TestRemoveProductFromCartSuccessfullyWhenThereAreMoreThanOneProduct()
        {
            await productsService.AddAsync(new AddProductInputModel
            {
                CategoryId = 1,
                Name = "TestProd",
                Price = 2M,
                Quantity = 1,
                Description = "Test Description",
            }, "TestUserId", string.Empty);

            await productsService.ApproveAsync(1, "TestAdminId");

            await productsService.AddAsync(new AddProductInputModel
            {
                CategoryId = 1,
                Name = "TestProd",
                Price = 3.5M,
                Quantity = 1,
                Description = "Test Description",
            }, "TestUserId", string.Empty);

            await productsService.ApproveAsync(2, "TestAdminId");


            await productsService.AddAsync(new AddProductInputModel
            {
                CategoryId = 1,
                Name = "TestProd",
                Price = 2M,
                Quantity = 1,
                Description = "Test Description",
            }, "TestUserId", string.Empty);

            await productsService.ApproveAsync(3, "TestAdminId");

            await cartsService.AddAsync("TestUserId2");

            await cartsService.AddProductAsync(1, 1, "TestUserId2");
            await cartsService.AddProductAsync(1, 2, "TestUserId2");
            await cartsService.AddProductAsync(1, 3, "TestUserId2");

            var result = await cartsService.RemoveProductAsync(1, 1, "TestUserId2");
            var model = result.Model;
            Assert.That(result.Result, Is.EqualTo(AddUpdateDeleteResult.Success));
            Assert.NotNull(model);
            Assert.That(model.ProductId, Is.EqualTo(1));
            Assert.That(model.NewCount, Is.EqualTo(2));
            Assert.That(model.NewTotalPrice, Is.EqualTo(5.5M));

            var result2 = await cartsService.RemoveProductAsync(1, 2, "TestUserId2");

            var model2 = result2.Model;
            Assert.That(result2.Result, Is.EqualTo(AddUpdateDeleteResult.Success));
            Assert.NotNull(model2);
            Assert.That(model2.ProductId, Is.EqualTo(2));
            Assert.That(model2.NewCount, Is.EqualTo(1));
            Assert.That(model2.NewTotalPrice, Is.EqualTo(2));

            var result3 = await cartsService.RemoveProductAsync(1, 3, "TestUserId2");

            var model3 = result3.Model;
            Assert.That(result3.Result, Is.EqualTo(AddUpdateDeleteResult.Success));
            Assert.NotNull(model3);
            Assert.That(model3.ProductId, Is.EqualTo(3));
            Assert.That(model3.NewCount, Is.EqualTo(0));
            Assert.That(model3.NewTotalPrice, Is.EqualTo(0));
        }

        [Test]
        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(-10)]
        public async Task TestRemoveAllProductsWhenCartIdIsNotAPositiveNumber(int cartId)
        {
            var result = await cartsService.RemoveAllProductsAsync(cartId, string.Empty);
            Assert.That(result, Is.EqualTo(AddUpdateDeleteResult.NotFound));
        }

        [Test]
        public async Task TestRemoveAllProductsWhenCartDoesNotExist()
        {
            var result = await cartsService.RemoveAllProductsAsync(1, string.Empty);
            Assert.That(result, Is.EqualTo(AddUpdateDeleteResult.NotFound));
        }

        [Test]
        public async Task TestRemoveAllProductsWhenCartExistsButThereAreNoProductsIn()
        {
            await cartsService.AddAsync("TestUserId");
            var result = await cartsService.RemoveAllProductsAsync(1, string.Empty);
            Assert.That(result, Is.EqualTo(AddUpdateDeleteResult.NotFound));
        }

        [Test]
        public async Task TestRemoveAllProductsWhenTriedToBeRemovedNotFromTheOwner()
        {
            await productsService.AddAsync(new AddProductInputModel
            {
                CategoryId = 1,
                Name = "TestProd",
                Price = 2M,
                Quantity = 1,
                Description = "Test Description",
            }, "TestUserId", string.Empty);

            await productsService.ApproveAsync(1, "TestAdminId");

            await cartsService.AddAsync("TestUserId2");

            await cartsService.AddProductAsync(1, 1, "TestUserId2");

            var result = await cartsService.RemoveAllProductsAsync(1, "TestUserId3");
            Assert.That(result, Is.EqualTo(AddUpdateDeleteResult.Forbidden));
        }


        [Test]
        public async Task TestRemoveAllProductsSuccessfully()
        {
            await productsService.AddAsync(new AddProductInputModel
            {
                CategoryId = 1,
                Name = "TestProd",
                Price = 2M,
                Quantity = 1,
                Description = "Test Description",
            }, "TestUserId", string.Empty);

            await productsService.ApproveAsync(1, "TestAdminId");

            await cartsService.AddAsync("TestUserId2");

            await cartsService.AddProductAsync(1, 1, "TestUserId2");

            var result = await cartsService.RemoveAllProductsAsync(1, "TestUserId2");
            Assert.That(result, Is.EqualTo(AddUpdateDeleteResult.Success));
        }

        [Test]
        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(-10)]
        public async Task TestUpdateQuantityWhenProductIdIsNotAPositiveNumber(int productId)
        {
            var result = await cartsService.UpdateQuantityInCartAsync(1, productId, 3, string.Empty);
            Assert.That(result.Result, Is.EqualTo(AddUpdateDeleteResult.NotFound));
        }

        [Test]
        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(-10)]
        public async Task TestUpdateQuantityWhenCartIdIsNotAPositiveNumber(int cartId)
        {
            var result = await cartsService.UpdateQuantityInCartAsync(cartId, 1, 3, string.Empty);
            Assert.That(result.Result, Is.EqualTo(AddUpdateDeleteResult.NotFound));
        }

        [Test]
        public async Task TestUpdateQuantityWhenCartDoesNotExist()
        {
            await productsService.AddAsync(new AddProductInputModel
            {
                CategoryId = 1,
                Name = "TestProd",
                Price = 2M,
                Quantity = 1,
                Description = "Test Description",
            }, "TestUserId", string.Empty);

            await productsService.ApproveAsync(1, "TestAdminId");

            var result = await cartsService.UpdateQuantityInCartAsync(1, 1, 3, string.Empty);
            Assert.That(result.Result, Is.EqualTo(AddUpdateDeleteResult.NotFound));
        }

        [Test]
        public async Task TestUpdateQuantityWhenProductDoesNotExist()
        {
            await cartsService.AddAsync("TestUserId");

            var result = await cartsService.UpdateQuantityInCartAsync(1, 1, 3, string.Empty);
            Assert.That(result.Result, Is.EqualTo(AddUpdateDeleteResult.NotFound));
        }

        [Test]
        public async Task TestUpdateQuantityWhenTriedToUpdateByNotTheOwner()
        {
            await cartsService.AddAsync("TestUserId2");
            await productsService.AddAsync(new AddProductInputModel
            {
                CategoryId = 1,
                Name = "TestProd",
                Price = 2M,
                Quantity = 1,
                Description = "Test Description",
            }, "TestUserId", string.Empty);

            await productsService.ApproveAsync(1, "TestAdminId");

            await cartsService.AddProductAsync(1, 1, "TestUserId2");

            var result = await cartsService.UpdateQuantityInCartAsync(1, 1, 3, "TestUserId3");
            Assert.That(result.Result, Is.EqualTo(AddUpdateDeleteResult.Forbidden));
        }

        [Test]
        public async Task TestUpdateQuantityWhenThereIsLessThanRequested()
        {
            await cartsService.AddAsync("TestUserId2");
            await productsService.AddAsync(new AddProductInputModel
            {
                CategoryId = 1,
                Name = "TestProd",
                Price = 2M,
                Quantity = 3,
                Description = "Test Description",
            }, "TestUserId", string.Empty);

            await productsService.ApproveAsync(1, "TestAdminId");

            await cartsService.AddProductAsync(1, 1, "TestUserId2");

            var result = await cartsService.UpdateQuantityInCartAsync(1, 1, 4, "TestUserId2");
            var model = result.Model;
            Assert.That(result.Result, Is.EqualTo(AddUpdateDeleteResult.Success));
            Assert.IsNotNull(model);
            Assert.IsTrue(model.IsThereLessThanRequested);
            Assert.That(model.NewTotalPrice, Is.EqualTo(6));
            Assert.That(model.UpdatedQuantity, Is.EqualTo(3));
            Assert.That(model.NewProductPrice, Is.EqualTo(6));
        }

        [Test]
        public async Task TestUpdateQuantityWhenThereIsMoreOrEqualThanRequested()
        {
            await cartsService.AddAsync("TestUserId2");
            await productsService.AddAsync(new AddProductInputModel
            {
                CategoryId = 1,
                Name = "TestProd",
                Price = 2M,
                Quantity = 5,
                Description = "Test Description",
            }, "TestUserId", string.Empty);

            await productsService.ApproveAsync(1, "TestAdminId");

            await cartsService.AddProductAsync(1, 1, "TestUserId2");

            var result = await cartsService.UpdateQuantityInCartAsync(1, 1, 4, "TestUserId2");
            var result2 = await cartsService.UpdateQuantityInCartAsync(1, 1, 5, "TestUserId2");
            var model = result.Model;
            var model2 = result2.Model;
            Assert.That(result.Result, Is.EqualTo(AddUpdateDeleteResult.Success));
            Assert.IsNotNull(model);
            Assert.IsFalse(model.IsThereLessThanRequested);
            Assert.That(model.NewTotalPrice, Is.EqualTo(8));
            Assert.That(model.UpdatedQuantity, Is.EqualTo(4));
            Assert.That(model.NewProductPrice, Is.EqualTo(8));

            Assert.That(result2.Result, Is.EqualTo(AddUpdateDeleteResult.Success));
            Assert.IsNotNull(model2);
            Assert.IsFalse(model2.IsThereLessThanRequested);
            Assert.That(model2.NewTotalPrice, Is.EqualTo(10));
            Assert.That(model2.UpdatedQuantity, Is.EqualTo(5));
            Assert.That(model2.NewProductPrice, Is.EqualTo(10));
        }

        [Test]
        public async Task TestUpdateQuantityWhenThereAreMultipleProducts()
        {
            await cartsService.AddAsync("TestUserId2");
            await productsService.AddAsync(new AddProductInputModel
            {
                CategoryId = 1,
                Name = "TestProd",
                Price = 2M,
                Quantity = 1,
                Description = "Test Description",
            }, "TestUserId", string.Empty);

            await productsService.ApproveAsync(1, "TestAdminId");


            await productsService.AddAsync(new AddProductInputModel
            {
                CategoryId = 1,
                Name = "TestProd",
                Price = 20.52M,
                Quantity = 2,
                Description = "Test Description",
            }, "TestUserId", string.Empty);

            await productsService.ApproveAsync(2, "TestAdminId");

            await productsService.AddAsync(new AddProductInputModel
            {
                CategoryId = 1,
                Name = "TestProd",
                Price = 10M,
                Quantity = 3,
                Description = "Test Description",
            }, "TestUserId", string.Empty);

            await productsService.ApproveAsync(3, "TestAdminId");
            await cartsService.AddProductAsync(1, 1, "TestUserId2");
            await cartsService.AddProductAsync(1, 2, "TestUserId2");
            await cartsService.AddProductAsync(1, 3, "TestUserId2");

            var result = await cartsService.UpdateQuantityInCartAsync(1, 3, 4, "TestUserId2");
            var model = result.Model;
            Assert.That(result.Result, Is.EqualTo(AddUpdateDeleteResult.Success));
            Assert.IsNotNull(model);
            Assert.IsTrue(model.IsThereLessThanRequested);
            Assert.That(model.NewTotalPrice, Is.EqualTo(52.52));
            Assert.That(model.UpdatedQuantity, Is.EqualTo(3));
            Assert.That(model.NewProductPrice, Is.EqualTo(30));

        }

        [Test]
        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(-10)]
        public async Task TestGetUserIdWhenCartIdIsNotAPositiveNumber(int cartId)
        {
            var result = await cartsService.GetUserIdAsync(cartId);
            Assert.IsNull(result);
        }

        [Test]
        public async Task TestGetUserIdWhenCartExists()
        {
            await cartsService.AddAsync("TestUserId");
            var result = await cartsService.GetUserIdAsync(1);
            Assert.IsNotNull(result);
            Assert.That(result, Is.EqualTo("TestUserId"));
        }


        [Test]
        public async Task TestBelongsToUserWhenTheUserIsNotTheOwner()
        {
            var result = await cartsService.BelongsToUserAsync("OwnerId", "NotOwnerId");
            Assert.IsFalse(result);
        }


        [Test]
        public async Task TestBelongsToUserWhenTheUserIsAdmin()
        {
            var result = await cartsService.BelongsToUserAsync("OwnerId", "TestAdminId");
            Assert.IsTrue(result);
        }

        [Test]
        public async Task TestBelongsToUserWhenTheUserIsTheOwner()
        {
            var result = await cartsService.BelongsToUserAsync("OwnerId", "OwnerId");
            Assert.IsTrue(result);
        }
    }


}
