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
using SimpleShoppingApp.Services.Emails;

namespace SimpleShoppingApp.Tests
{
    public class ProductsServiceTests
    {
        private ApplicationDbContext db;
        private IRepository<Product> productsRepository;
        private IRepository<Category> categoryRepository;
        private IRepository<ApplicationUser> userRepository;
        private IRepository<UsersRating> usersRatingRepository;
        private IRepository<CartsProducts> cartsProductsRepository;
        private IProductsService productsService;
        private ICategoriesService categoriesService;
        private IUsersService usersService;
        private IImagesService imagesService;
        private INameShortenerService shortenerService;
        private IEmailsService emailService;

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
            cartsProductsRepository = new Repository<CartsProducts>(db);
            categoriesService = new CategoriesService(categoryRepository);

            await categoryRepository.AddAsync(new Category
            {
                Name = "Category",
            });

            await userRepository.AddAsync(new ApplicationUser
            {
                Id = "UserId",
                Email = "testuser@test.test",
                UserName = "testuser@test.test",
            });

            await userRepository.AddAsync(new ApplicationUser
            {
                Id = "UserId2",
                Email = "testuser2@test.test",
                UserName = "testuser2@test.test",
            });

            await categoryRepository.SaveChangesAsync();

            var imagesServiceMock = new Mock<IImagesService>();

            var usersServiceMock = new Mock<IUsersService>();

            var shortenerMock = new Mock<INameShortenerService>();

            var emailServiceMock = new Mock<IEmailsService>();

            usersServiceMock.Setup(x => x.IsInRoleAsync("AdministatorID", "Administrator"))
                .ReturnsAsync(true);

            usersServiceMock.Setup(x => x.IsInRoleAsync("UserId", "Administrator"))
                .ReturnsAsync(false);

            usersServiceMock.Setup(x => x.GetAdminIdAsync())
            .ReturnsAsync("AdminId");

            usersServiceMock.Setup(x => x.GetEmailAsync("UserId"))
                .ReturnsAsync(string.Empty);

            imagesServiceMock.Setup(x => x.GetAsync(1))
                .ReturnsAsync(new List<ImageViewModel>());

            for (int i = 1; i <= 10; i++)
            {
                imagesServiceMock.Setup(x => x.GetFirstAsync(i))
                   .ReturnsAsync((ImageViewModel)null);
            };

            shortenerMock.Setup(x => x.Shorten("TestProd", 50))
                .Returns("Test");


            emailServiceMock.Setup(x => x.SendAsync(string.Empty, string.Empty, string.Empty, string.Empty))
                .ReturnsAsync(true);

            usersService = usersServiceMock.Object;
            imagesService = imagesServiceMock.Object;
            shortenerService = shortenerMock.Object;
            emailService = emailServiceMock.Object;


            productsService = new ProductsService(productsRepository, usersRatingRepository, cartsProductsRepository, imagesService, categoriesService, usersService, shortenerService, emailService);
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
                Description = "Test Description",
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

            await productsService.ApproveAsync(1, "AdminId");

            var result = await productsService.GetAsync(1, null);

            Assert.IsNotNull(result);

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

            await productsService.ApproveAsync(1, "AdminId");

            var product = await productsService.GetAsync(1, null);

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

            await productsService.ApproveAsync(1, "AdminId");

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

            await productsService.ApproveAsync(1, "AdminId");

            await productsService.ApproveAsync(2, "AdminId");

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

                await productsService.ApproveAsync(i + 1, "AdminId");
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

                await productsService.ApproveAsync(i + 1, "AdminId");
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


                await productsService.ApproveAsync(result.ProductId ?? 0, "AdminId");

                if (filter != RatingFilter.Zero)
                {

                    var ratingsList = kvp.Value;

                    await productsService.AddRatingFromUserAsync(result.ProductId ?? 0, "UserId3", ratingsList[0], null);
                    await productsService.AddRatingFromUserAsync(result.ProductId ?? 0, "UserId2", ratingsList[1], null);
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

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public async Task TestApproveProductWhenProductDoesNotExist(int productId)
        {
            var result = await productsService.ApproveAsync(productId, "AdminId");
            Assert.IsFalse(result);
        }

        [Test]
        public async Task TestUnApproveProductWhenNotAdminTriesToApprove()
        {
            await productsService.AddAsync(new AddProductInputModel
            {
                CategoryId = 1,
                Name = "TestProd",
                Price = 1,
                Quantity = 2,
                Description = "Test Desc",
            }, "UserId", string.Empty);

            await productsService.AddAsync(new AddProductInputModel
            {
                CategoryId = 1,
                Name = "TestProd",
                Price = 1,
                Quantity = 2,
                Description = "Test Desc",
            }, "UserId", string.Empty);

            var resultOne = await productsService.UnApproveAsync(1, "UserId");
            var resultTwo = await productsService.UnApproveAsync(2, "UserId");
            Assert.IsFalse(resultOne);
            Assert.IsFalse(resultTwo);


        }

        [Test]
        public async Task TestUnApproveProductWhenProductExists()
        {
            await productsService.AddAsync(new AddProductInputModel
            {
                CategoryId = 1,
                Name = "TestProd",
                Price = 1,
                Quantity = 2,
                Description = "Test Desc",
            }, "UserId", string.Empty);

            await productsService.AddAsync(new AddProductInputModel
            {
                CategoryId = 1,
                Name = "TestProd",
                Price = 1,
                Quantity = 2,
                Description = "Test Desc",
            }, "UserId", string.Empty);

            var resultOne = await productsService.UnApproveAsync(1, "AdminId");
            var resultTwo = await productsService.UnApproveAsync(2, "AdminId");
            Assert.IsTrue(resultOne);
            Assert.IsTrue(resultTwo);


        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public async Task TestUnApproveProductWhenProductDoesNotExist(int productId)
        {
            var result = await productsService.UnApproveAsync(productId, "AdminId");
            Assert.IsFalse(result);
        }

        [Test]
        public async Task TestApproveProductWhenNotAnAdminTriesToApprove()
        {
            await productsService.AddAsync(new AddProductInputModel
            {
                CategoryId = 1,
                Name = "TestProd",
                Price = 1,
                Quantity = 2,
                Description = "Test Desc",
            }, "UserId", string.Empty);

            await productsService.AddAsync(new AddProductInputModel
            {
                CategoryId = 1,
                Name = "TestProd",
                Price = 1,
                Quantity = 2,
                Description = "Test Desc",
            }, "UserId", string.Empty);

            var resultOne = await productsService.ApproveAsync(1, "UserId");
            var resultTwo = await productsService.ApproveAsync(2, "UserId");
            Assert.IsFalse(resultOne);
            Assert.IsFalse(resultTwo);

        }

        [Test]
        public async Task TestApproveProductWhenProductExists()
        {
            await productsService.AddAsync(new AddProductInputModel
            {
                CategoryId = 1,
                Name = "TestProd",
                Price = 1,
                Quantity = 2,
                Description = "Test Desc",
            }, "UserId", string.Empty);

            await productsService.AddAsync(new AddProductInputModel
            {
                CategoryId = 1,
                Name = "TestProd",
                Price = 1,
                Quantity = 2,
                Description = "Test Desc",
            }, "UserId", string.Empty);

            var resultOne = await productsService.ApproveAsync(1, "AdminId");
            var resultTwo = await productsService.ApproveAsync(2, "AdminId");
            Assert.IsTrue(resultOne);
            Assert.IsTrue(resultTwo);

        }

        [Test]
        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(-100)]
        public async Task TestDeleteWhenIdIsNotAPositiveNumber(int id)
        {
            await productsService.AddAsync(new AddProductInputModel
            {
                CategoryId = 1,
                Name = "TestProd",
                Price = 1,
                Quantity = 2,
                Description = "Test Desc",
            }, "UserId", string.Empty);

            await productsService.ApproveAsync(1, "AdminId");

            var result = await productsService.DeleteAsync(id, string.Empty);
            Assert.That(result, Is.EqualTo(AddUpdateDeleteResult.NotFound));

        }

        [Test]
        public async Task TestDeleteWhenProductToDeleteIsNotFound()
        {
            await productsService.AddAsync(new AddProductInputModel
            {
                CategoryId = 1,
                Name = "TestProd",
                Price = 1,
                Quantity = 2,
                Description = "Test Desc",
            }, "UserId", string.Empty);

            await productsService.ApproveAsync(1, "AdminId");

            var result = await productsService.DeleteAsync(2, string.Empty);

            Assert.That(result, Is.EqualTo(AddUpdateDeleteResult.NotFound));
        }

        [Test]
        public async Task TestDeleteWhenUserTryingToDeleteItIsNotTheOwner()
        {
            await productsService.AddAsync(new AddProductInputModel
            {
                CategoryId = 1,
                Name = "TestProd",
                Price = 1,
                Quantity = 2,
                Description = "Test Desc",
            }, "UserId", string.Empty);

            await productsService.ApproveAsync(1, "AdminId");

            var result = await productsService.DeleteAsync(1, "UserId2");

            Assert.That(result, Is.EqualTo(AddUpdateDeleteResult.Forbidden));
        }

        [Test]
        public async Task TestSuccessfulDelete()
        {
            await productsService.AddAsync(new AddProductInputModel
            {
                CategoryId = 1,
                Name = "TestProd",
                Price = 1,
                Quantity = 2,
                Description = "Test Desc",
            }, "UserId", string.Empty);

            await productsService.ApproveAsync(1, "AdminId");

            var result = await productsService.DeleteAsync(1, "UserId");

            Assert.That(result, Is.EqualTo(AddUpdateDeleteResult.Success));
        }

        [Test]
        public async Task TestSuccessfulDeleteFromAdmin()
        {
            await productsService.AddAsync(new AddProductInputModel
            {
                CategoryId = 1,
                Name = "TestProd",
                Price = 1,
                Quantity = 2,
                Description = "Test Desc",
            }, "UserId", string.Empty);

            await productsService.ApproveAsync(1, "AdminId");

            var result = await productsService.DeleteAsync(1, "AdminId");

            Assert.That(result, Is.EqualTo(AddUpdateDeleteResult.Success));
        }



        [Test]
        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(-100)]
        public async Task TestCategoryCountWhenCategoryIdIsNotAPositiveNumber(int categoryId)
        {
            var count = await productsService.GetCountForCategoryAsync(categoryId);
            Assert.That(count, Is.EqualTo(0));
        }

        [Test]
        public async Task TestCategoryCountWhenCategoryDoesNoExist()
        {
            var count = await productsService.GetCountForCategoryAsync(300);
            Assert.That(count, Is.EqualTo(0));
        }

        [Test]
        public async Task TestCategoryCountWhenThereAreNoProducts()
        {
            var count = await productsService.GetCountForCategoryAsync(1);
            Assert.That(count, Is.EqualTo(0));
        }

        [Test]
        public async Task TestCategoryCountWhenThereAreProducts()
        {
            for (int i = 0; i < 10; i++)
            {
                await productsService.AddAsync(new AddProductInputModel
                {
                    CategoryId = 1,
                    Name = "TestProd",
                    Price = 1,
                    Quantity = 2,
                    Description = "Test Desc",
                }, "UserId", string.Empty);

                await productsService.ApproveAsync(i + 1, "AdminId");
            }

            var count = await productsService.GetCountForCategoryAsync(1);

            Assert.That(count, Is.EqualTo(10));
        }

        [Test]
        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(-10)]
        public async Task TestGetToEditWhenProductIdIsNotAPositiveNumber(int productId)
        {
            var result = await productsService.GetToEditAsync(productId, string.Empty);
            Assert.That(result.Result, Is.EqualTo(AddUpdateDeleteResult.NotFound));
        }

        [Test]
        public async Task TestGetToEditWhenProductDoesNotExist()
        {
            var result = await productsService.GetToEditAsync(1, string.Empty);
            Assert.That(result.Result, Is.EqualTo(AddUpdateDeleteResult.NotFound));
        }

        [Test]
        public async Task TestGetToEditWhenUserTryingToEditIsNotTheOwner()
        {
            await productsService.AddAsync(new AddProductInputModel
            {
                CategoryId = 1,
                Name = "TestProd",
                Price = 1,
                Quantity = 2,
                Description = "Test Desc",
            }, "UserId", string.Empty);

            await productsService.ApproveAsync(1, "AdminId");

            var result = await productsService.GetToEditAsync(1, "UserId2");
            Assert.That(result.Result, Is.EqualTo(AddUpdateDeleteResult.Forbidden));

        }

        [Test]
        public async Task TestGetToEditSuccessfully()
        {
            await productsService.AddAsync(new AddProductInputModel
            {
                CategoryId = 1,
                Name = "TestProd",
                Price = 1,
                Quantity = 2,
                Description = "Test Desc",
            }, "UserId", string.Empty);

            await productsService.ApproveAsync(1, "AdminId");

            var result = await productsService.GetToEditAsync(1, "UserId");
            Assert.That(result.Result, Is.EqualTo(AddUpdateDeleteResult.Success));

        }


        [Test]
        public async Task TestGetToEditByAdmin()
        {
            await productsService.AddAsync(new AddProductInputModel
            {
                CategoryId = 1,
                Name = "TestProd",
                Price = 1,
                Quantity = 2,
                Description = "Test Desc",
            }, "UserId", string.Empty);

            await productsService.ApproveAsync(1, "AdminId");

            var result = await productsService.GetToEditAsync(1, "AdminId");
            Assert.That(result.Result, Is.EqualTo(AddUpdateDeleteResult.Success));

        }

        [Test]
        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(-10)]
        public async Task TestUpdateWhenProductIdIsNotAPositiveNumber(int productId)
        {
            var result = await productsService.UpdateAsync(new EditProductInputModel
            {
                Id = productId,
                CategoryId = 1,

            }, string.Empty);

            Assert.That(result, Is.EqualTo(AddUpdateDeleteResult.NotFound));
        }

        [Test]
        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(-10)]
        public async Task TestUpdateWhenCategoryIdIsNotAPositiveNumber(int categoryId)
        {
            var result = await productsService.UpdateAsync(new EditProductInputModel
            {
                Id = 1,
                CategoryId = categoryId,

            }, string.Empty);

            Assert.That(result, Is.EqualTo(AddUpdateDeleteResult.NotFound));
        }

        [Test]
        public async Task TestUpdateWhenCategoryDoesNotExist()
        {
            var result = await productsService.UpdateAsync(new EditProductInputModel
            {
                Id = 1,
                CategoryId = 2,

            }, string.Empty);

            Assert.That(result, Is.EqualTo(AddUpdateDeleteResult.NotFound));
        }

        [Test]
        public async Task TestUpdateWhenProductDoesNotExist()
        {
            var result = await productsService.UpdateAsync(new EditProductInputModel
            {
                Id = 1,
                CategoryId = 1,

            }, string.Empty);

            Assert.That(result, Is.EqualTo(AddUpdateDeleteResult.NotFound));
        }

        [Test]
        public async Task TestUpdateWhenUserTryingToUpdateItIsNotTheOwner()
        {
            await productsService.AddAsync(new AddProductInputModel
            {
                CategoryId = 1,
                Name = "TestProd",
                Price = 1,
                Quantity = 2,
                Description = "Test Desc",
            }, "UserId", string.Empty);

            await productsService.ApproveAsync(1, "AdminId");

            var result = await productsService.UpdateAsync(new EditProductInputModel
            {
                Id = 1,
                CategoryId = 1,

            }, "UserId2");

            Assert.That(result, Is.EqualTo(AddUpdateDeleteResult.Forbidden));

        }

        [Test]
        public async Task TestUpdateByAdmin()
        {
            await productsService.AddAsync(new AddProductInputModel
            {
                CategoryId = 1,
                Name = "TestProd",
                Price = 1,
                Quantity = 2,
                Description = "Test Desc",
            }, "UserId", string.Empty);

            await productsService.ApproveAsync(1, "AdminId");

            var result = await productsService.UpdateAsync(new EditProductInputModel
            {
                Id = 1,
                CategoryId = 1,
                Name = "UpdatedTestProd"

            }, "AdminId");

            Assert.That(result, Is.EqualTo(AddUpdateDeleteResult.Success));
        }

        [Test]
        public async Task TestUpdateSuccessfully()
        {
            await productsService.AddAsync(new AddProductInputModel
            {
                CategoryId = 1,
                Name = "TestProd",
                Price = 1,
                Quantity = 2,
                Description = "Test Desc",
            }, "UserId", string.Empty);

            await productsService.ApproveAsync(1, "AdminId");

            var result = await productsService.UpdateAsync(new EditProductInputModel
            {
                Id = 1,
                CategoryId = 1,
                Name = "UpdatedTestProd"

            }, "UserId");

            Assert.That(result, Is.EqualTo(AddUpdateDeleteResult.Success));
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        public async Task TestGetByNameWhenNameIsEmpty(string? name)
        {
            var products = await productsService.GetByNameAsync(name);
            Assert.That(products.Count(), Is.EqualTo(0));
        }

        [Test]
        public async Task TestGetByNameWhenTheNameIsValid()
        {
            await productsService.AddAsync(new AddProductInputModel
            {
                CategoryId = 1,
                Name = "TestProd",
                Price = 1,
                Quantity = 2,
                Description = "Test Desc",
            }, "UserId", string.Empty);

            await productsService.AddAsync(new AddProductInputModel
            {
                CategoryId = 1,
                Name = "Prod",
                Price = 1,
                Quantity = 2,
                Description = "Test Desc",
            }, "UserId", string.Empty);

            await productsService.AddAsync(new AddProductInputModel
            {
                CategoryId = 1,
                Name = "ProdTest",
                Price = 1,
                Quantity = 2,
                Description = "Test Desc",
            }, "UserId", string.Empty);

            await productsService.ApproveAsync(1, "AdminId");
            await productsService.ApproveAsync(2, "AdminId");
            await productsService.ApproveAsync(3, "AdminId");

            var products = await productsService.GetByNameAsync("Prod");

            Assert.That(products.Count(), Is.EqualTo(3));
        }

        [Test]
        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(-10)]
        public async Task TestBelongsToUserWhenProductIdIsNotAPositiveNumber(int productId)
        {
            var result = await productsService.BelognsToUserAsync(productId, string.Empty);
            Assert.IsFalse(result);
        }

        [Test]
        public async Task TestBelongsToUserWhenProductDoesNotExist()
        {
            var result = await productsService.BelognsToUserAsync(1, string.Empty);
            Assert.IsFalse(result);
        }


        [Test]
        public async Task TestBelongsToUserWhenLoggedInUserIsNotTheOwner()
        {
            await productsService.AddAsync(new AddProductInputModel
            {
                CategoryId = 1,
                Name = "TestProd",
                Price = 1,
                Quantity = 2,
                Description = "Test Desc",
            }, "UserId", string.Empty);

            await productsService.ApproveAsync(1, "AdminId");

            var result = await productsService.BelognsToUserAsync(1, "UserId2");
            Assert.IsFalse(result);
        }

        [Test]
        public async Task TestBelongsToUserWhenLoggedInUserIsTheOwner()
        {
            await productsService.AddAsync(new AddProductInputModel
            {
                CategoryId = 1,
                Name = "TestProd",
                Price = 1,
                Quantity = 2,
                Description = "Test Desc",
            }, "UserId", string.Empty);

            await productsService.ApproveAsync(1, "AdminId");

            var result = await productsService.BelognsToUserAsync(1, "UserId");
            Assert.IsTrue(result);
        }

        [Test]
        public async Task TestBelongsToUserWhenLoggedInUserIsTheAdmin()
        {
            await productsService.AddAsync(new AddProductInputModel
            {
                CategoryId = 1,
                Name = "TestProd",
                Price = 1,
                Quantity = 2,
                Description = "Test Desc",
            }, "UserId", string.Empty);

            await productsService.ApproveAsync(1, "AdminId");

            var result = await productsService.BelognsToUserAsync(1, "AdminId");
            Assert.IsTrue(result);
        }

        [Test]
        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(-10)]
        public async Task TestGetQuantityWhenProductIdIsNotAPositiveNumber(int productId)
        {
            var result = await productsService.GetQuantityAsync(productId);
            Assert.IsNull(result);
        }

        [Test]
        public async Task TestGetQuantityWhenProductDoesNotExist()
        {
            var result = await productsService.GetQuantityAsync(1);
            Assert.IsNull(result);
        }

        [Test]
        public async Task TestGetQuantityWhenProductExists()
        {
            await productsService.AddAsync(new AddProductInputModel
            {
                CategoryId = 1,
                Name = "TestProd",
                Price = 1,
                Quantity = 2,
                Description = "Test Desc",
            }, "UserId", string.Empty);

            await productsService.ApproveAsync(1, "AdminId");

            var result = await productsService.GetQuantityAsync(1);

            Assert.NotNull(result);

            Assert.That(result, Is.EqualTo(2));
        }

        [Test]
        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(-10)]
        public async Task TestDoesProductExistWhenProductIdIsNotAPositiveNumber(int productId)
        {
            var result = await productsService.DoesProductExistAsync(productId);
            Assert.IsFalse(result);
        }

        [Test]
        public async Task TestDoesProductExistWhenProductDoesNotExist()
        {
            var result = await productsService.DoesProductExistAsync(1);
            Assert.IsFalse(result);
        }

        [Test]
        public async Task TestDoesProductExistWhenProductExists()
        {
            await productsService.AddAsync(new AddProductInputModel
            {
                CategoryId = 1,
                Name = "TestProd",
                Price = 1,
                Quantity = 2,
                Description = "Test Desc",
            }, "UserId", string.Empty);

            await productsService.ApproveAsync(1, "AdminId");

            var result = await productsService.DoesProductExistAsync(1);
            Assert.IsTrue(result);
        }

        [Test]
        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(-100)]
        public async Task TestAddRatingFromUserWhenProductIdIsNotAPositiveNumber(int productId)
        {
            var result = await productsService.AddRatingFromUserAsync(productId, string.Empty, 1, null);
            Assert.That(result.Result, Is.EqualTo(AddUpdateDeleteResult.NotFound));
        }

        [Test]
        public async Task TestAddRatingFromUserWhenProductDoesNotExist()
        {
            var result = await productsService.AddRatingFromUserAsync(1, string.Empty, 1, null);
            Assert.That(result.Result, Is.EqualTo(AddUpdateDeleteResult.NotFound));
        }

        [Test]
        [TestCase(0)]
        [TestCase(6)]
        public async Task TestAddRatingFromUserWhenRatingIsInvalid(int rating)
        {
            await productsService.AddAsync(new AddProductInputModel
            {
                CategoryId = 1,
                Name = "TestProd",
                Price = 1,
                Quantity = 2,
                Description = "Test Desc",
            }, "UserId", string.Empty);

            await productsService.ApproveAsync(1, "AdminId");
            var result = await productsService.AddRatingFromUserAsync(1, string.Empty, rating, null);
            Assert.That(result.Result, Is.EqualTo(AddUpdateDeleteResult.NotFound));
        }

        [Test]
        public async Task TestAddRatingSuccessfullyWhenThereIsOneRating()
        {
            await productsService.AddAsync(new AddProductInputModel
            {
                CategoryId = 1,
                Name = "TestProd",
                Price = 1,
                Quantity = 2,
                Description = "Test Desc",
            }, "UserId", string.Empty);

            await productsService.ApproveAsync(1, "AdminId");

            var result = await productsService.AddRatingFromUserAsync(1, "UserId2", 3, null);

            Assert.That(result.Result, Is.EqualTo(AddUpdateDeleteResult.Success));
            Assert.That(result.AvgRating, Is.EqualTo(3));

        }


        [Test]
        public async Task TestAddRatingSuccessfullyWhenThereAreMoreThanOneRating()
        {
            await productsService.AddAsync(new AddProductInputModel
            {
                CategoryId = 1,
                Name = "TestProd",
                Price = 1,
                Quantity = 2,
                Description = "Test Desc",
            }, "UserId", string.Empty);

            await productsService.ApproveAsync(1, "AdminId");

            var result = await productsService.AddRatingFromUserAsync(1, "UserId3", 3, null);
            var result2 = await productsService.AddRatingFromUserAsync(1, "UserId2", 4, null);
            var result3 = await productsService.AddRatingFromUserAsync(1, "UserId2", 5, null);


            Assert.That(result.Result, Is.EqualTo(AddUpdateDeleteResult.Success));
            Assert.That(result.AvgRating, Is.EqualTo(3));

            Assert.That(result2.Result, Is.EqualTo(AddUpdateDeleteResult.Success));
            Assert.That(result2.AvgRating, Is.EqualTo(3.5));

            Assert.That(result3.Result, Is.EqualTo(AddUpdateDeleteResult.Success));
            Assert.That(result3.AvgRating, Is.EqualTo(4));

        }

        [Test]
        public async Task TestAddRatingSuccessfullyWhenUserChangesRating()
        {
            await productsService.AddAsync(new AddProductInputModel
            {
                CategoryId = 1,
                Name = "TestProd",
                Price = 1,
                Quantity = 2,
                Description = "Test Desc",
            }, "UserId", string.Empty);

            await productsService.ApproveAsync(1, "AdminId");

            var result = await productsService.AddRatingFromUserAsync(1, "UserId2", 3, null);
            var result2 = await productsService.AddRatingFromUserAsync(1, "UserId2", 4, null);


            Assert.That(result.Result, Is.EqualTo(AddUpdateDeleteResult.Success));
            Assert.That(result.AvgRating, Is.EqualTo(3));

            Assert.That(result2.Result, Is.EqualTo(AddUpdateDeleteResult.Success));
            Assert.That(result2.AvgRating, Is.EqualTo(4));

        }

        [Test]
        public async Task GetReviewTextWhenProductDoesNotExist()
        {
            var result = await productsService.GetReviewTextAsync("UserId", 1);
            Assert.IsNull(result);
        }

        [Test]
        public async Task GetReviewTextWhenUserDoesNotExist()
        {
            await productsService.AddAsync(new AddProductInputModel
            {
                CategoryId = 1,
                Name = "TestProd",
                Price = 1,
                Quantity = 2,
                Description = "Test Desc",
            }, "UserId", string.Empty);

            await productsService.ApproveAsync(1, "AdminId");
            var result = await productsService.GetReviewTextAsync("UserId2323", 1);
            Assert.IsNull(result);
        }

        [Test]
        public async Task GetReviewTextSuccessfully()
        {
            await productsService.AddAsync(new AddProductInputModel
            {
                CategoryId = 1,
                Name = "TestProd",
                Price = 1,
                Quantity = 2,
                Description = "Test Desc",
            }, "UserId", string.Empty);

            await productsService.ApproveAsync(1, "AdminId");
            await productsService.AddRatingFromUserAsync(1, "UserId2", 3, "testtext");
            var result = await productsService.GetReviewTextAsync("UserId2", 1);
            Assert.NotNull(result);
            Assert.That(result, Is.EqualTo("testtext"));
        }

        [Test]
        public async Task TestGetReviewsWhenProductDoesNotExist()
        {
            var result = await productsService.GetReviewsAsync(1, "testuser@test.test");
            Assert.That(result.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task TestGetReviewsWhenProductExistsButThereArentAnyReviews()
        {
            await productsService.AddAsync(new AddProductInputModel
            {
                CategoryId = 1,
                Name = "TestProd",
                Price = 1,
                Quantity = 2,
                Description = "Test Desc",
            }, "UserId", string.Empty);

            await productsService.ApproveAsync(1, "AdminId");

            var result = await productsService.GetReviewsAsync(1, "testuser@test.test");
            Assert.That(result.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task TestGetReviewsSuccessfullyByNotLoggedIn()
        {
            await productsService.AddAsync(new AddProductInputModel
            {
                CategoryId = 1,
                Name = "TestProd",
                Price = 1,
                Quantity = 2,
                Description = "Test Desc",
            }, "UserId", string.Empty);

            await productsService.ApproveAsync(1, "AdminId");

            await productsService.AddRatingFromUserAsync(1, "UserId2", 3, "testtext");

            var result = await productsService.GetReviewsAsync(1, null);
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result.First().UserName, Is.EqualTo("testuser2@test.test"));
            Assert.That(result.First().Rating, Is.EqualTo(3));
            Assert.That(result.First().Text, Is.EqualTo("testtext"));
            Assert.IsFalse(result.First().IsMine);
        }

        [Test]
        public async Task TestGetReviewsSuccessfullyByLoggedIn()
        {
            await productsService.AddAsync(new AddProductInputModel
            {
                CategoryId = 1,
                Name = "TestProd",
                Price = 1,
                Quantity = 2,
                Description = "Test Desc",
            }, "UserId", string.Empty);

            await productsService.ApproveAsync(1, "AdminId");

            await productsService.AddRatingFromUserAsync(1, "UserId2", 3, "testtext");

            var result = await productsService.GetReviewsAsync(1, "testuser2@test.test");
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result.First().UserName, Is.EqualTo("testuser2@test.test"));
            Assert.That(result.First().Rating, Is.EqualTo(3));
            Assert.That(result.First().Text, Is.EqualTo("testtext"));
            Assert.IsTrue(result.First().IsMine);
        }

        [Test]
        public async Task TestGetCountWhenThereAreNoApprovedProducts()
        {
            for (int i = 0; i < 10; i++)
            {
                await productsService.AddAsync(new AddProductInputModel
                {
                    CategoryId = 1,
                    Name = "TestProd",
                    Price = 1,
                    Quantity = 2,
                    Description = "Test Desc",
                }, "UserId", string.Empty);
            }

            var count = await productsService.GetCountAsync();
            Assert.That(count, Is.EqualTo(0));
        }

        [Test]
        public async Task TestGetCountSuccessfully()
        {
            for (int i = 0; i < 10; i++)
            {
                await productsService.AddAsync(new AddProductInputModel
                {
                    CategoryId = 1,
                    Name = "TestProd",
                    Price = 1,
                    Quantity = 2,
                    Description = "Test Desc",
                }, "UserId", string.Empty);

                await productsService.ApproveAsync(i+1, "AdminId");

            }

            var count = await productsService.GetCountAsync();
            Assert.That(count, Is.EqualTo(10));
        }

        [Test]
        public async Task TestGetOwnerIdWhenProductDoesNotExist()
        {
            var result = await productsService.GetOwnerIdAsync(1);
            Assert.IsNull(result);
        }


        [Test]
        public async Task TestGetOwnerIdWhenProductNotApproved()
        {
            await productsService.AddAsync(new AddProductInputModel
            {
                CategoryId = 1,
                Name = "TestProd",
                Price = 1,
                Quantity = 2,
                Description = "Test Desc",
            }, "UserId", string.Empty);

            var result = await productsService.GetOwnerIdAsync(1);
            Assert.IsNotNull(result);
            Assert.That(result, Is.EqualTo("UserId"));
        }

        [Test]
        public async Task TestGetOwnerIdWhenExists()
        {
            await productsService.AddAsync(new AddProductInputModel
            {
                CategoryId = 1,
                Name = "TestProd",
                Price = 1,
                Quantity = 2,
                Description = "Test Desc",
            }, "UserId", string.Empty);

            await productsService.ApproveAsync(1, "AdminId");

            var result = await productsService.GetOwnerIdAsync(1);
            Assert.IsNotNull(result);
            Assert.That(result, Is.EqualTo("UserId"));
        }

    }
}
