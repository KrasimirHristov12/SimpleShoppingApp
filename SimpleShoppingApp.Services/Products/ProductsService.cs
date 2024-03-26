using LinqKit;
using Microsoft.EntityFrameworkCore;
using SimpleShoppingApp.Data.Enums;
using SimpleShoppingApp.Data.Models;
using SimpleShoppingApp.Data.Repository;
using SimpleShoppingApp.Models.Images;
using SimpleShoppingApp.Models.Products;
using SimpleShoppingApp.Services.Categories;
using SimpleShoppingApp.Services.Images;
using SimpleShoppingApp.Services.Notifications;
using SimpleShoppingApp.Services.Users;

namespace SimpleShoppingApp.Services.Products
{
    public class ProductsService : IProductsService
    {
        private readonly IRepository<Product> productsRepo;
        private readonly IRepository<UsersRating> usersRatingRepo;
        private readonly IImagesService imagesService;
        private readonly ICategoriesService categoriesService;
        private readonly IUsersService usersService;
        private readonly INotificationsService notificationsService;

        public ProductsService(
            IRepository<Product> _productsRepo,
            IRepository<UsersRating> _usersRatingRepo,
            IImagesService _imagesService,
            ICategoriesService _categoriesService,
            IUsersService _usersService,
            INotificationsService _notificationsService)
        {
            productsRepo = _productsRepo;
            usersRatingRepo = _usersRatingRepo;
            imagesService = _imagesService;
            categoriesService = _categoriesService;
            usersService = _usersService;
            notificationsService = _notificationsService;
        }

        public async Task<AddProductModel> AddAsync(AddProductInputModel model, string userId, string imagesDirectory)
        {
            if (model.CategoryId <= 0)
            {
                return new AddProductModel
                {
                    Result = AddUpdateDeleteResult.NotFound,
                    ProductId = null,
                };
            }

            if (!await categoriesService.DoesCategoryExist(model.CategoryId))
            {
                return new AddProductModel
                {
                    Result = AddUpdateDeleteResult.NotFound,
                    ProductId = null,
                };
            }

            bool isApproved = await usersService.IsInRoleAsync(userId, "Administrator");

            var product = new Product()
            {
                Name = model.Name,
                Description = model.Description,
                IsApproved = isApproved,
                Price = model.Price ?? 0,
                Quantity = model.Quantity ?? 0,
                Rating = 0,
                CategoryId = model.CategoryId,
                UserId = userId,
            };

            await productsRepo.AddAsync(product);

            if (model.Images != null)
            {
                await productsRepo.SaveChangesAsync();

                foreach (var imageFromModel in model.Images.Where(i => i != null))
                {
                    string imageUID = Guid.NewGuid().ToString();

                    string imageName = $"prod{product.Id}_{imageUID}";

                    string wwwrootDir = imagesDirectory;

                    await imagesService.AddAsync(imageFromModel, imageName, wwwrootDir, product.Id);

                }
            }

            if (model.ImagesUrls != null)
            {
                foreach (var imageUrl in model.ImagesUrls.Where(i => !string.IsNullOrWhiteSpace(i)))
                {
                    product.Images.Add(new Image
                    {
                        ImageUrl = imageUrl
                    });
                }
            }

            await productsRepo.SaveChangesAsync();

            if (!product.IsApproved)
            {
                string? adminUserId = await usersService.GetAdminIdAsync();
                if (!string.IsNullOrWhiteSpace(adminUserId))
                {
                    bool notificationsResult = await notificationsService.AddAsync(userId, adminUserId, "A new product was added for approval.", $"/Administration/Products/Approve/{product.Id}");
                }
                
            }

            return new AddProductModel
            {
                Result = AddUpdateDeleteResult.Success,
                ProductId = product.Id,
            };
        }

        public async Task<ProductViewModel?> GetAsync(int id, string? userId)
        {
            if (id <= 0)
            {
                return null;
            }

            var product = await productsRepo.AllAsNoTracking()
                .Where(p => p.Id == id && !p.IsDeleted)
                .Select(p => new ProductViewModel
                {
                    Id = id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    Quantity = p.Quantity,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.Name,
                    UserName = p.User.UserName,
                    HasDiscount = p.HasDiscount,
                    NewPrice = p.NewPrice,
                })
               .FirstOrDefaultAsync();

            if (product == null)
            {
                return null;
            }

            var images = await imagesService.GetAsync(id);

            product.Images = images;

            if (userId == null)
            {
                product.BelongsToCurrentUser = false;
            }
            else
            {
                product.BelongsToCurrentUser = await BelognsToUserAsync(id, userId);
                var rating = await GetRatingAsync(id, userId);
                product.Rating = rating;
                product.AvgRating = await GetAverageRatingAsync(id);
            }

            return product;
        }

        public async Task<IEnumerable<ListProductsViewModel>> GetRandomProductsAsync(int n)
        {
            if (n <= 0)
            {
                return new List<ListProductsViewModel>();
            }

            var products = await productsRepo.AllAsNoTracking()
                .Where(p => !p.IsDeleted)
                .OrderBy(p => Guid.NewGuid())
                .Take(n)
                .Select(p => new ListProductsViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Rating = p.Rating,

                })
                .ToListAsync();

            foreach (var product in products)
            {
                var image = await imagesService.GetFirstAsync(product.Id);
                if (image != null)
                {
                    product.Image = image;
                }

            }

            return products;
        }

        public async Task<ProductsPerPageModel?> GetByCategoryAsync(ProductsFilterModel model)
        {
            if (model.CategoryId <= 0 || model.ProductsPerPage <= 0 || model.Page <= 0)
            {
                return null;
            }

            if (!await categoriesService.DoesCategoryExist(model.CategoryId))
            {
                return null;
            }

            List<ListProductsViewModel> products = new List<ListProductsViewModel>();

            var productsQuery = productsRepo.AllAsNoTracking()
            .Where(p => p.CategoryId == model.CategoryId && !p.IsDeleted);

            if (model.Prices.Count() == 0 && model.Ratings.Count() == 0)
            {
                products =  await productsQuery
                .Skip((model.Page - 1) * model.ProductsPerPage)
                .Take(model.ProductsPerPage)
                .Select(p => new ListProductsViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Rating = p.Rating,

                }).ToListAsync();
            }

            else
            {
                var predicatePrice = PredicateBuilder.New<Product>();
                var predicateRating = PredicateBuilder.New<Product>();

                foreach (var priceFilter in model.Prices)
                {
                    predicatePrice = priceFilter switch
                    {
                        PriceFilter.OneToFifty => predicatePrice.Or(p => p.Price >= 1 && p.Price <= 50),
                        PriceFilter.FiftyOneToTwoHundred => predicatePrice.Or(p => p.Price >= 51 && p.Price <= 200),
                        PriceFilter.TwoHundredOneToFiveHundred => predicatePrice.Or(p => p.Price >= 201 && p.Price <= 500),
                        PriceFilter.FiveHundredOneToOneThousand => predicatePrice.Or(p => p.Price >= 501 && p.Price <= 1000),
                        PriceFilter.OneThousandToOneThousandFourHundredNinetyNine => predicatePrice.Or(p => p.Price >= 1000 && p.Price <= 1499),
                        PriceFilter.OneThousandFiveHundredOrMore => predicatePrice.Or(p => p.Price >= 1500),
                        _ => predicatePrice,
                    };
                }
                if (predicatePrice.IsStarted)
                {
                    productsQuery = productsQuery.Where(predicatePrice);
                }

                var ratings = model.Ratings.ToList();

                foreach (var ratingFilter in model.Ratings)
                {
                    predicateRating = ratingFilter switch
                    {
                        RatingFilter.Zero => predicateRating.Or(p => p.Rating >= 0 && p.Rating < 1),
                        RatingFilter.One => predicateRating.Or(p => p.Rating >= 1 && p.Rating < 2),
                        RatingFilter.Two => predicateRating.Or(p => p.Rating >= 2 && p.Rating < 3),
                        RatingFilter.Three => predicateRating.Or(p => p.Rating >= 3 && p.Rating < 4),
                        RatingFilter.Four => predicateRating.Or(p => p.Rating >= 4 && p.Rating < 5),
                        RatingFilter.Five => predicateRating.Or(p => p.Rating >= 5),
                        _ => predicateRating,
                    };
                }
                if (predicateRating.IsStarted)
                {
                    productsQuery = productsQuery.Where(predicateRating);
                }


                products = await productsQuery
                    .Skip((model.Page - 1) * model.ProductsPerPage)
                    .Take(model.ProductsPerPage)
                    .Select(p => new ListProductsViewModel
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Price = p.Price,
                        Rating = p.Rating,
                    })
                    .ToListAsync();

            }


            foreach (var product in products)
            {
                var image = await imagesService.GetFirstAsync(product.Id);
                if (image != null)
                {
                    product.Image = image;
                }
            }

            int totalElements = await productsQuery.CountAsync();

            var modelToReturn = new ProductsPerPageModel
            {
                Products = products,
                TotalPages = (int)Math.Ceiling((decimal)totalElements / model.ProductsPerPage),
                TotalProducts = totalElements,
            };

            return modelToReturn;
        }

        public async Task<AddUpdateDeleteResult> DeleteAsync(int id, string currentUserId)
        {
            if (id <= 0)
            {
                return AddUpdateDeleteResult.NotFound;
            }

            var productToDelete = await productsRepo.AllAsTracking()
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

            if (productToDelete == null)
            {
                return AddUpdateDeleteResult.NotFound;
            }

            if (productToDelete.UserId != currentUserId)
            {
                return AddUpdateDeleteResult.Forbidden;
            }

            productToDelete.IsDeleted = true;
            await productsRepo.SaveChangesAsync();
            return AddUpdateDeleteResult.Success;

        }

        public async Task<int> GetCountForCategoryAsync(int categoryId)
        {
            if (categoryId <= 0)
            {
                return 0;
            }

            return await productsRepo
                .AllAsNoTracking()
                .Where(p => p.CategoryId == categoryId && !p.IsDeleted)
                .CountAsync();
        }

        public async Task<EditProductModel> GetToEditAsync(int id, string currentUserId)
        {
            if (id <= 0)
            {
                return new EditProductModel
                {
                    Result = AddUpdateDeleteResult.NotFound,
                    Model = null,
                };
            }

            var productToEdit = await productsRepo.AllAsNoTracking()
                .Where(p => p.Id == id && !p.IsDeleted)
                .Select(p => new EditProductInputModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    CategoryId = p.CategoryId,
                    Description = p.Description,
                    Quantity = p.Quantity,
                    HasDiscount = p.HasDiscount,
                    NewPrice = p.NewPrice,

                }).FirstOrDefaultAsync();

            if (productToEdit == null)
            {
                return new EditProductModel
                {
                    Result = AddUpdateDeleteResult.NotFound,
                    Model = null,
                };
            }

            if (!await BelognsToUserAsync(id, currentUserId))
            {
                return new EditProductModel
                {
                    Result = AddUpdateDeleteResult.Forbidden,
                    Model = null,
                };
            }

            productToEdit.Categories = await categoriesService.GetAllAsync();

            return new EditProductModel
            {
                Result = AddUpdateDeleteResult.Success,
                Model = productToEdit,
            };
        }

        public async Task<ApproveProductViewModel?> GetProductToApproveAsync(int productId)
        {
            var product = await productsRepo.AllAsNoTracking()
                .Where(p => p.Id == productId && !p.IsDeleted)
                .Select(p => new ApproveProductViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    CategoryName = p.Category.Name,
                    Description = p.Description,
                    Images = p.Images.Select(i => new ImageViewModel
                    {
                        Extension = i.Extension,
                        ImageUrl = i.ImageUrl,
                        Name = i.Name,

                    }).ToList(),
                })
                .FirstOrDefaultAsync();

            return product;

        }

        public async Task<AddUpdateDeleteResult> UpdateAsync(EditProductInputModel model, string currentUserId)
        {
            if (model.Id <= 0 || model.CategoryId <= 0)
            {
                return AddUpdateDeleteResult.NotFound;
            }

            if (!await categoriesService.DoesCategoryExist(model.CategoryId))
            {
                return AddUpdateDeleteResult.NotFound;
            }

            var productToEdit = await productsRepo
                .AllAsTracking()
                .FirstOrDefaultAsync(p => p.Id == model.Id && !p.IsDeleted);

            if (productToEdit == null)
            {
                return AddUpdateDeleteResult.NotFound;
            }

            if (!await BelognsToUserAsync(model.Id, currentUserId))
            {
                return AddUpdateDeleteResult.Forbidden;
            }

            productToEdit.Name = model.Name;
            productToEdit.Price = model.Price ?? 0;
            productToEdit.Quantity = model.Quantity ?? 0;
            productToEdit.CategoryId = model.CategoryId;
            productToEdit.Description = model.Description;
            productToEdit.HasDiscount = model.HasDiscount;
            productToEdit.NewPrice = model.NewPrice;

            await productsRepo.SaveChangesAsync();

            return AddUpdateDeleteResult.Success;
        }

        public async Task<IEnumerable<ListProductsViewModel>> GetByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return new List<ListProductsViewModel>();
            }

            var filteredProducts = await productsRepo.AllAsNoTracking()
                .Where(p => p.Name.ToLower().Contains(name.ToLower()) && !p.IsDeleted)
                .Select(p => new ListProductsViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Rating = p.Rating,
                }).ToListAsync();

            foreach (var product in filteredProducts)
            {
                var image = await imagesService.GetFirstAsync(product.Id);
                if (image != null)
                {
                    product.Image = image;
                }
            }
            return filteredProducts;
        }

        public async Task<bool> BelognsToUserAsync(int productId, string loggedInUserId)
        {
            if (productId <= 0)
            {
                return false;
            }
            var creatorUserId = await productsRepo
                .AllAsNoTracking()
                .Where(p => p.Id == productId && !p.IsDeleted)
                .Select(p => p.UserId)
                .FirstOrDefaultAsync();

            if (creatorUserId == null)
            {
                return false;
            }

            if (loggedInUserId == creatorUserId)
            {
                return true;
            }

            string? adminUserId = await usersService.GetAdminIdAsync();

            if (adminUserId == null)
            {
                return false;
            }

            if (loggedInUserId == adminUserId)
            {
                return true;
            }

            return false;

        }

        public async Task<int?> GetQuantityAsync(int id)
        {
            if (id <= 0)
            {
                return null;
            }

            return await productsRepo
                .AllAsNoTracking()
                .Where(p => p.Id == id && !p.IsDeleted)
                .Select(p => p.Quantity)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> DoesProductExistAsync(int productId)
        {
            if (productId <= 0)
            {
                return false;
            }
            return await productsRepo
                .AllAsNoTracking()
                .AnyAsync(p => p.Id == productId && !p.IsDeleted);
        }

        public async Task<ProductRatingViewModel> AddRatingFromUserAsync(int productId, string loggedInUserId, int rating)
        {
            if (productId <= 0)
            {
                return new ProductRatingViewModel
                {
                    Result = AddUpdateDeleteResult.NotFound,
                    AvgRating = null,
                };
            }

            var product = await productsRepo
            .AllAsTracking()
            .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null)
            {
                return new ProductRatingViewModel
                {
                    Result = AddUpdateDeleteResult.NotFound,
                    AvgRating = null,
                };
            }

            if (rating < 1 || rating > 5)
            {
                return new ProductRatingViewModel
                {
                    Result = AddUpdateDeleteResult.NotFound,
                    AvgRating = null,
                };
            }

            var userProductRating = await usersRatingRepo
                .AllAsTracking()
                .FirstOrDefaultAsync(ur => ur.UserId == loggedInUserId && ur.ProductId == productId);

            if (userProductRating == null)
            {
                userProductRating = new UsersRating
                {
                    UserId = loggedInUserId,
                    ProductId = productId,
                    Rating = rating,
                };

                await usersRatingRepo.AddAsync(userProductRating);
                await usersRatingRepo.SaveChangesAsync();
                var averageRating = await GetAverageRatingAsync(productId);
                product.Rating = averageRating;
                await productsRepo.SaveChangesAsync();
                return new ProductRatingViewModel
                {
                    AvgRating = averageRating,
                    Result = AddUpdateDeleteResult.Success,
                };
            }

            userProductRating.Rating = rating;
            await usersRatingRepo.SaveChangesAsync();
            var avgRating = await GetAverageRatingAsync(productId);
            return new ProductRatingViewModel
            {
                AvgRating = avgRating,
                Result = AddUpdateDeleteResult.Success,
            };
        }

        private async Task<int> GetRatingAsync(int productId, string userId)
        {
            var rating = await usersRatingRepo
                .AllAsNoTracking()
                .Where(ur => ur.ProductId == productId && ur.UserId == userId)
                .Select(ur => ur.Rating)
                .FirstOrDefaultAsync();

            return rating;
        }

        private async Task<double> GetAverageRatingAsync(int productId)
        {
            var allRatings = await usersRatingRepo
                .AllAsNoTracking()
                .Where(ur => ur.ProductId == productId)
                .Select(ur => ur.Rating)
                .ToListAsync();

            var avgRating = allRatings.Count > 0 ? allRatings.Average() : 0;

            return avgRating;
        }

        public async Task<int> GetCountAsync()
        {
            return await productsRepo.AllAsNoTracking()
                .Where(p => !p.IsDeleted)
                .CountAsync();
        }
    }
}
