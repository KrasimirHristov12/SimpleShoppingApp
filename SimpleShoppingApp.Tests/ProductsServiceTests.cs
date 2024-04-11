using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using SimpleShoppingApp.Data;
using SimpleShoppingApp.Data.Models;
using SimpleShoppingApp.Data.Repository;
using SimpleShoppingApp.Services.Categories;
using SimpleShoppingApp.Services.Products;
using SimpleShoppingApp.Services.Users;
using SimpleShoppingApp.Models.Products;
using SimpleShoppingApp.Data.Enums;
using SimpleShoppingApp.Services.Images;
using SimpleShoppingApp.Models.Images;
using SimpleShoppingApp.Services.NameShortener;

namespace SimpleShoppingApp.Tests
{
    public class ProductsServiceTests
    {
        private ApplicationDbContext db;
        private IRepository<Product> productsRepository;
        private IRepository<Category> categoryRepository;
        private IRepository<ApplicationUser> userRepository;
        private IRepository<UsersRating> usersRatingRepository;
        private IProductsService productsService;
        private ICategoriesService categoriesService;
        private IUsersService usersService;
        private IImagesService imagesService;
        private INameShortenerService shortenerService;

        [SetUp]
        public async Task Initialize()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
           .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
           .Options;

            db = new ApplicationDbContext(options);
            productsRepository = new Repository<Product>(db);
            categoryRepository = new Repository<Category>(db);
            userRepository = new Repository<ApplicationUser>(db);
            usersRatingRepository = new Repository<UsersRating>(db);
            categoriesService = new CategoriesService(categoryRepository);

            await categoryRepository.AddAsync(new Category
            {
                Name = "Category",
            });

            await userRepository.AddAsync(new ApplicationUser
            {
                Id = "UserId",
                Email = "testuser@test.test",
            });

            await userRepository.AddAsync(new ApplicationUser
            {
                Id = "UserId2",
                Email = "testuser2@test.test",
            });

            await categoryRepository.SaveChangesAsync();

            //categoriesServiceMock.Setup(x => x.DoesCategoryExistAsync(1))
            //    .ReturnsAsync(true);

            //categoriesServiceMock.Setup(x => x.DoesCategoryExistAsync(2))
            //    .ReturnsAsync(false);

            var imagesServiceMock = new Mock<IImagesService>();

            var usersServiceMock = new Mock<IUsersService>();

            var shortenerMock = new Mock<INameShortenerService>();

            usersServiceMock.Setup(x => x.IsInRoleAsync("AdministatorID", "Administrator"))
                .ReturnsAsync(true);

            usersServiceMock.Setup(x => x.IsInRoleAsync("UserId", "Administrator"))
                .ReturnsAsync(false);

            imagesServiceMock.Setup(x => x.GetAsync(1))
                .ReturnsAsync(new List<ImageViewModel>());

            for (int i = 1; i <= 10; i++)
            {
                imagesServiceMock.Setup(x => x.GetFirstAsync(i))
                   .ReturnsAsync((ImageViewModel)null);
            }

            

            shortenerMock.Setup(x => x.Shorten("TestProd", 50))
                .Returns("Test");

            usersService = usersServiceMock.Object;
            imagesService = imagesServiceMock.Object;
            shortenerService = shortenerMock.Object;


            productsService = new ProductsService(productsRepository, usersRatingRepository, imagesService, categoriesService, usersService, null, shortenerService);
        }

        [Test]
        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(-100)]
        public async Task TestAddingAProductWhenCategoryIdIsNotAPositiveNumber(int categoryId)
        {
            var result = await productsService.AddAsync(new AddProductInputModel
            {
                CategoryId = categoryId,
                Name = "TestProd",
            }, string.Empty, string.Empty);

            Assert.That(result.Result, Is.EqualTo(AddUpdateDeleteResult.NotFound));
        }

        [Test]
        public async Task TestAddingAProductWhenCategoryDoesNotExist()
        {
            var result = await productsService.AddAsync(new AddProductInputModel
            {
                CategoryId = 2,
                Name = "TestProd",
            }, string.Empty, string.Empty);

            Assert.That(result.Result, Is.EqualTo(AddUpdateDeleteResult.NotFound));
        }

        [Test]
        public async Task TestSuccessfulAddingOfProduct()
        {
            var result = await productsService.AddAsync(new AddProductInputModel
            {
                CategoryId = 1,
                Name = "TestProd",
                Price = 2M,
                Quantity = 2,
                Description = "Test Description"
            }, "UserId", string.Empty);

            Assert.That(result.Result, Is.EqualTo(AddUpdateDeleteResult.Success));
        }

        [Test]
        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(-100)]
        public async Task TestGetProductWhenProductIdIsNotAPositiveNumber(int productId)
        {
            var result = await productsService.GetAsync(productId, string.Empty);
            Assert.IsNull(result);
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public async Task TestGetProductWhenProductDoesNotExist(int productId)
        {
            var result = await productsService.GetAsync(productId, "UserId");
            Assert.IsNull(result);
        }

        [Test]
        public async Task TestGetProductAnonymousUser()
        {
            await productsService.AddAsync(new AddProductInputModel
            {
                CategoryId = 1,
                Name = "TestProd",
                Price = 2M,
                Quantity = 2,
                Description = "Test Description"
            }, "UserId", string.Empty);

            var result = await productsService.GetAsync(1, null) ?? new ProductViewModel();

            Assert.IsFalse(result.BelongsToCurrentUser);
        }

        [Test]
        public async Task TestSuccessfulGet()
        {
            await productsService.AddAsync(new AddProductInputModel
            {
                CategoryId = 1,
                Name = "TestProd",
                Price = 2M,
                Quantity = 2,
                Description = "Test Description"
            }, "UserId", string.Empty);

            var product = await productsService.GetAsync(1, "UserId");

            Assert.IsNotNull(product);
        }

        [Test]
        public async Task TestGetProductNameIfProductDoesNotExist()
        {
            var productName = await productsService.GetNameAsync(1);
            Assert.IsNull(productName);
        }

        [Test]
        public async Task TestGetProductNameSuccessfully()
        {
           await productsService.AddAsync(new AddProductInputModel
            {
                CategoryId = 1,
                Name = "TestProd",
                Price = 2M,
                Quantity = 2,
                Description = "Test Description"
            }, "UserId", string.Empty);

            var productName = await productsService.GetNameAsync(1);

            Assert.That(productName, Is.EqualTo("TestProd"));
        }

        [Test]
        public async Task TestGetRandomProductWithNoProducts()
        {
            var products = await productsService.GetRandomProductsAsync(2);

            Assert.That(products.Count(), Is.EqualTo(0));
        }

        [Test]
        public async Task TestGetRandomProductsCountWithTwoProducts()
        {
            await productsService.AddAsync(new AddProductInputModel
            {
                CategoryId = 1,
                Name = "TestProd",
                Price = 2M,
                Quantity = 2,
                Description = "Test Description"
            }, "UserId", string.Empty);

            await productsService.AddAsync(new AddProductInputModel
            {
                CategoryId = 1,
                Name = "TestProd2",
                Price = 2M,
                Quantity = 2,
                Description = "Test Description2"
            }, "UserId", string.Empty);

            var products = await productsService.GetRandomProductsAsync(2);

            Assert.That(products.Count(), Is.EqualTo(2));
        }

        [Test]
        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(-100)]
        public async Task TestGetByCategoryWhenCategoryIdIsNotAPositiveNumber(int categoryId)
        {
            var result = await productsService.GetByCategoryAsync(new ProductsFilterModel
            {
                CategoryId = categoryId,
                ProductsPerPage = 1,
                Page = 1,
            });

            Assert.IsNull(result);
        }

        [Test]
        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(-100)]
        public async Task TestGetByCategoryWhenProductsPerPageIsNotAPositiveNumber(int productsPerPage)
        {
            var result = await productsService.GetByCategoryAsync(new ProductsFilterModel
            {
                CategoryId = 1,
                ProductsPerPage = productsPerPage,
                Page = 1,
            });

            Assert.IsNull(result);
        }

        [Test]
        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(-100)]
        public async Task TestGetByCategoryWhenPageIsNotAPositiveNumber(int page)
        {
            var result = await productsService.GetByCategoryAsync(new ProductsFilterModel
            {
                CategoryId = 1,
                ProductsPerPage = 1,
                Page = page,
            });

            Assert.IsNull(result);
        }

        [Test]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        public async Task TestGetByCategoryWhenCategoryDoesNotExist(int categoryId)
        {
            var result = await productsService.GetByCategoryAsync(new ProductsFilterModel
            {
                CategoryId = categoryId,
                ProductsPerPage = 1,
                Page = 1,
            });

            Assert.IsNull(result);

        }

        [Test]
        public async Task TestGetByCategoryWhenThereAreNoFilters()
        {
            for (int i = 0; i < 10; i++)
            {
                await productsService.AddAsync(new AddProductInputModel
                {
                    CategoryId = 1,
                    Name = "TestProd",
                    Price = 2M,
                    Quantity = 2,
                    Description = "Test Description2"
                }, "UserId", string.Empty);
            }

            var result = await productsService.GetByCategoryAsync(new ProductsFilterModel
            {
                CategoryId = 1,
                Page = 1,
                ProductsPerPage = 5,
            });

            Assert.NotNull(result);

            Assert.That(result.TotalProducts, Is.EqualTo(10));

            Assert.That(result.TotalPages, Is.EqualTo(2));

            Assert.That(result.Products.Count(), Is.EqualTo(5));
        }

        [Test]
        [TestCase(PriceFilter.ZeroToFifty)]
        [TestCase(PriceFilter.FiftyToTwoHundred)]
        [TestCase(PriceFilter.TwoHundredToFiveHundred)]
        [TestCase(PriceFilter.FiveHundredToNineHundredNinetyNine)]
        [TestCase(PriceFilter.NineHundredNinetyNineToOneThousandFourHundredNinetyNine)]
        [TestCase(PriceFilter.OverOneThousandFourHundredNinetyNine)]
        public async Task TestGetByCategoryWithPriceFilters(PriceFilter filter)
        {
            List<decimal> prices = new List<decimal>
            {
                1, 2.01M, 50, 51, 199.99M, 200, 200.01M, 201, 499.99M, 500, 500.01M, 501, 989.99M, 999.99M, 1000, 1000.01M, 1499, 1500, 1501, 10000
            };

            for (int i = 0; i < prices.Count; i++)
            {
                await productsService.AddAsync(new AddProductInputModel
                {
                    CategoryId = 1,
                    Name = "TestProd",
                    Price = prices[i],
                    Quantity = 2,
                    Description = "Test Desc",
                }, "UserId", string.Empty);
            }

            var filteredProducts = await productsService.GetByCategoryAsync(new ProductsFilterModel
            {
                CategoryId = 1,
                Prices = new List<PriceFilter> { filter },
                Page = 1,
                ProductsPerPage = 1,
            });

            Assert.NotNull(filteredProducts);

            if (filter == PriceFilter.ZeroToFifty)
            {
                Assert.That(filteredProducts.TotalProducts, Is.EqualTo(3));
            }
            else if (filter == PriceFilter.FiftyToTwoHundred)
            {
                Assert.That(filteredProducts.TotalProducts, Is.EqualTo(3));
            }

            else if (filter == PriceFilter.TwoHundredToFiveHundred)
            {
                Assert.That(filteredProducts.TotalProducts, Is.EqualTo(4));
            }

            else if (filter == PriceFilter.FiveHundredToNineHundredNinetyNine)
            {
                Assert.That(filteredProducts.TotalProducts, Is.EqualTo(3));
            }

            else if (filter == PriceFilter.NineHundredNinetyNineToOneThousandFourHundredNinetyNine)
            {
                Assert.That(filteredProducts.TotalProducts, Is.EqualTo(4));
            }

            else if (filter == PriceFilter.OverOneThousandFourHundredNinetyNine)
            {
                Assert.That(filteredProducts.TotalProducts, Is.EqualTo(3));
            }

        }

        [Test]
        [TestCase(RatingFilter.Zero)]
        [TestCase(RatingFilter.One)]
        [TestCase(RatingFilter.Two)]
        [TestCase(RatingFilter.Three)]
        [TestCase(RatingFilter.Four)]
        [TestCase(RatingFilter.Five)]
        public async Task TestGetByCategoryWithRatingFilters(RatingFilter filter)
        {
            Dictionary<double, List<int>> ratings = new Dictionary<double, List<int>>()
            {
                {1, new List<int> {1, 1}},
                {1.5, new List<int> {1, 2}},
                {2, new List<int> {2, 2}},
                {2.5, new List<int> {2, 3}},
                {3, new List<int> {3, 3}},
                {3.5, new List<int> {3, 4}},
                {4, new List<int> {4, 4}},
                {4.5, new List<int> {5, 4}},
                {5, new List<int> {5, 5}},

            };

            foreach (var kvp in ratings)
            {
                var result = await productsService.AddAsync(new AddProductInputModel
                {
                    CategoryId = 1,
                    Name = "TestProd",
                    Price = 1,
                    Quantity = 2,
                    Description = "Test Desc",
                }, "UserId", string.Empty);


                if (filter != RatingFilter.Zero)
                {

                    var ratingsList = kvp.Value;

                    await productsService.AddRatingFromUserAsync(result.ProductId ?? 1, "UserId", ratingsList[0]);
                    await productsService.AddRatingFromUserAsync(result.ProductId ?? 1, "UserId2", ratingsList[1]);
                }

            }

            var filteredProducts = await productsService.GetByCategoryAsync(new ProductsFilterModel
            {
                CategoryId = 1,
                Ratings = new List<RatingFilter> { filter },
                Page = 1,
                ProductsPerPage = 1,
            });

            Assert.IsNotNull(filteredProducts);

            if (filter == RatingFilter.Zero)
            {
                Assert.That(filteredProducts.TotalProducts, Is.EqualTo(ratings.Count));
            }
            else if (filter == RatingFilter.One || filter == RatingFilter.Two || filter == RatingFilter.Three || filter == RatingFilter.Four)
            {
                Assert.That(filteredProducts.TotalProducts, Is.EqualTo(2));
            }

            else if (filter == RatingFilter.Five)
            {
                Assert.That(filteredProducts.TotalProducts, Is.EqualTo(1));
            }


        }


    }
}
