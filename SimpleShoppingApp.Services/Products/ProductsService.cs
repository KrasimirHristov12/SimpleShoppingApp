using LinqKit;
using Microsoft.EntityFrameworkCore;
using SimpleShoppingApp.Data.Enums;
using SimpleShoppingApp.Data.Models;
using SimpleShoppingApp.Data.Repository;
using SimpleShoppingApp.Models.Products;
using SimpleShoppingApp.Services.Categories;
using SimpleShoppingApp.Services.Emails;
using SimpleShoppingApp.Services.Images;
using SimpleShoppingApp.Services.NameShortener;
using SimpleShoppingApp.Services.Notifications;
using SimpleShoppingApp.Services.Users;

namespace SimpleShoppingApp.Services.Products
{
    public class ProductsService : IProductsService
    {
        private readonly IRepository<Product> productsRepo;
        private readonly IRepository<UsersRating> usersRatingRepo;
        private readonly IRepository<CartsProducts> cartsProductsRepo;
        private readonly IImagesService imagesService;
        private readonly ICategoriesService categoriesService;
        private readonly IUsersService usersService;
        private readonly INotificationsService notificationsService;
        private readonly INameShortenerService shortenerService;
        private readonly IEmailsService emailsService;

        public ProductsService(
            IRepository<Product> _productsRepo,
            IRepository<UsersRating> _usersRatingRepo,
            IRepository<CartsProducts> _cartsProductsRepo,
            IImagesService _imagesService,
            ICategoriesService _categoriesService,
            IUsersService _usersService,
            INotificationsService _notificationsService,
            INameShortenerService _shortenerService,
            IEmailsService _emailsService)
        {
            productsRepo = _productsRepo;
            usersRatingRepo = _usersRatingRepo;
            cartsProductsRepo = _cartsProductsRepo;
            imagesService = _imagesService;
            categoriesService = _categoriesService;
            usersService = _usersService;
            notificationsService = _notificationsService;
            shortenerService = _shortenerService;
            emailsService = _emailsService;
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

            if (!await categoriesService.DoesCategoryExistAsync(model.CategoryId))
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
                    bool notificationsResult = await notificationsService.AddAsync(userId, adminUserId, "A new product was added for approval.", $"/Products/Index/{product.Id}");
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

            var productQuery = GetNotDeletedProducts();

            bool isUserAdmin = false;

            if (userId != null)
            {
                isUserAdmin = await usersService.IsInRoleAsync(userId, "Administrator");

                productQuery = isUserAdmin ? productQuery.Where(p => p.Id == id) : GetApprovedProducts().Where(p => p.Id == id);
            }
            else
            {
                productQuery = GetApprovedProducts().Where(p => p.Id == id);
            }

            var product = await productQuery
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
                    IsApproved = p.IsApproved,
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
                
                product.IsUserAdmin = isUserAdmin;
                product.BelongsToCurrentUser = await BelognsToUserAsync(id, userId);
                var rating = await GetRatingAsync(id, userId);
                product.Rating = rating;
                product.AvgRating = await GetAverageRatingAsync(id);
            }

            return product;
        }

        public async Task<string?> GetNameAsync(int productId)
        {
            var productName = await GetApprovedProducts()
                .Where(p => p.Id == productId)
                .Select(p => p.Name)
                .FirstOrDefaultAsync();

            return productName;
        }

        public async Task<IEnumerable<ListProductsViewModel>> GetRandomProductsAsync(int n)
        {
            if (n <= 0)
            {
                return new List<ListProductsViewModel>();
            }

            var products = await GetApprovedProducts()
                .OrderBy(p => Guid.NewGuid())
                .Take(n)
                .Select(p => new ListProductsViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Rating = p.Rating,
                    HasDiscount = p.HasDiscount,
                    NewPrice = p.NewPrice,

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

            if (!await categoriesService.DoesCategoryExistAsync(model.CategoryId))
            {
                return null;
            }

            var productsQuery = GetApprovedProducts()
            .Where(p => p.CategoryId == model.CategoryId);

            if (model.Prices.Count() > 0 || model.Ratings.Count() > 0)
            {
                var predicatePrice = PredicateBuilder.New<Product>();
                var predicateRating = PredicateBuilder.New<Product>();

                foreach (var priceFilter in model.Prices)
                {
                    predicatePrice = priceFilter switch
                    {
                        PriceFilter.ZeroToFifty => predicatePrice.Or(p => p.Price > 0 && p.Price <= 50),
                        PriceFilter.FiftyToTwoHundred => predicatePrice.Or(p => p.Price > 50 && p.Price <= 200),
                        PriceFilter.TwoHundredToFiveHundred => predicatePrice.Or(p => p.Price > 200 && p.Price <= 500),
                        PriceFilter.FiveHundredToNineHundredNinetyNine => predicatePrice.Or(p => p.Price > 500 && p.Price <= 999),
                        PriceFilter.NineHundredNinetyNineToOneThousandFourHundredNinetyNine => predicatePrice.Or(p => p.Price > 999 && p.Price <= 1499),
                        PriceFilter.OverOneThousandFourHundredNinetyNine => predicatePrice.Or(p => p.Price > 1499),
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

            }

            var products = await productsQuery
                .Skip((model.Page - 1) * model.ProductsPerPage)
                .Take(model.ProductsPerPage)
                .Select(p => new ListProductsViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Rating = p.Rating,
                    HasDiscount = p.HasDiscount,
                    NewPrice = p.NewPrice,
                })
                .ToListAsync();


            foreach (var product in products)
            {
                product.Name = shortenerService.Shorten(name: product.Name);
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

            var productToDelete = await GetApprovedProductsAsTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (productToDelete == null)
            {
                return AddUpdateDeleteResult.NotFound;
            }

            if (productToDelete.UserId != currentUserId)
            {
                return AddUpdateDeleteResult.Forbidden;
            }

            productToDelete.IsDeleted = true;

            var cartProducts = cartsProductsRepo.AllAsTracking()
                .Where(cp => cp.ProductId == id).ToList();

            foreach (var cartProduct in cartProducts)
            {
                cartProduct.IsDeleted = true;
            }

            await productsRepo.SaveChangesAsync();
            return AddUpdateDeleteResult.Success;

        }

        public async Task<int> GetCountForCategoryAsync(int categoryId)
        {
            if (categoryId <= 0)
            {
                return 0;
            }

            return await GetApprovedProducts()
                .Where(p => p.CategoryId == categoryId)
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

            var productToEdit = await GetApprovedProducts()
                .Where(p => p.Id == id)
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


        public async Task<AddUpdateDeleteResult> UpdateAsync(EditProductInputModel model, string currentUserId)
        {
            if (model.Id <= 0 || model.CategoryId <= 0)
            {
                return AddUpdateDeleteResult.NotFound;
            }

            if (!await categoriesService.DoesCategoryExistAsync(model.CategoryId))
            {
                return AddUpdateDeleteResult.NotFound;
            }

            var productToEdit = await GetApprovedProductsAsTracking()
                .FirstOrDefaultAsync(p => p.Id == model.Id);

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

            var filteredProducts = await GetApprovedProducts()
                .Where(p => p.Name.ToLower().Contains(name.ToLower()))
                .Select(p => new ListProductsViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Rating = p.Rating,
                    HasDiscount = p.HasDiscount,
                    NewPrice = p.NewPrice,
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
            var creatorUserId = await GetApprovedProducts()
                .Where(p => p.Id == productId)
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

            return await GetApprovedProducts()
                .Where(p => p.Id == id)
                .Select(p => p.Quantity as int?)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> DoesProductExistAsync(int productId)
        {
            if (productId <= 0)
            {
                return false;
            }
            return await GetApprovedProducts()
                .AnyAsync(p => p.Id == productId);
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

            var product = await GetApprovedProductsAsTracking()
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
            product.Rating = avgRating;
            await productsRepo.SaveChangesAsync();

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
            return await GetApprovedProducts()
                .CountAsync();
        }

        public async Task<string?> GetOwnerIdAsync(int productId)
        {
            var ownerId = await GetNotDeletedProducts()
                .Where(p => p.Id == productId)
                .Select(p => p.UserId)
                .FirstOrDefaultAsync();

            if (ownerId == null)
            {
                return null;
            }

            return ownerId;
        }

        public async Task<bool> ApproveAsync(int productId, string userId)
        {
            var product = await GetNotDeletedProductsAsTracking()
                .FirstOrDefaultAsync(p => p.Id == productId);
            if (product == null)
            {
                return false;
            }
            var adminUserId = await usersService.GetAdminIdAsync();
            var ownerUserId = await GetOwnerIdAsync(productId);

            if (string.IsNullOrWhiteSpace(adminUserId) || string.IsNullOrWhiteSpace(ownerUserId))
            {
                return false;
            }

            if (userId != adminUserId)
            {
                return false;
            }

            var emailOfOwner = await usersService.GetEmailAsync(ownerUserId);

            if (!string.IsNullOrWhiteSpace(emailOfOwner))
            {
                await emailsService.SendAsync(emailOfOwner, string.Empty, "Product approved", $"A product with id = {productId} has been approved from the administrator");
            }

            await notificationsService.AddAsync(adminUserId, ownerUserId, "A product you have created has been approved by the administrator", $"/Products/Index/{productId}");

            product.IsApproved = true;
            await productsRepo.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UnApproveAsync(int productId, string userId)
        {
            var product = await GetNotDeletedProductsAsTracking()
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null)
            {
                return false;
            }

            var adminUserId = await usersService.GetAdminIdAsync();
            var ownerUserId = await GetOwnerIdAsync(productId);

            if (string.IsNullOrWhiteSpace(adminUserId) || string.IsNullOrWhiteSpace(ownerUserId))
            {
                return false;
            }


            if (userId != adminUserId)
            {
                return false;
            }


            var emailOfOwner = await usersService.GetEmailAsync(ownerUserId);

            if (!string.IsNullOrWhiteSpace(emailOfOwner))
            {
                await emailsService.SendAsync(emailOfOwner, string.Empty, "Product not approved", $"A product with id = {productId} has NOT been approved from the administrator");
            }

            await notificationsService.AddAsync(adminUserId, ownerUserId, "A product you have created has not been approved by the administrator");

            product.IsDeleted = true;

            await productsRepo.SaveChangesAsync();

            return true;
        }

        private IQueryable<Product> GetNotDeletedProducts()
        {
            return productsRepo.AllAsNoTracking()
                .Where(p => !p.IsDeleted);
        }

        private IQueryable<Product> GetNotDeletedProductsAsTracking()
        {
            return productsRepo.AllAsTracking()
                .Where(p => !p.IsDeleted);
        }

        private IQueryable<Product> GetApprovedProducts()
        {
            return productsRepo.AllAsNoTracking()
            .Where(p => !p.IsDeleted && p.IsApproved);
        }
        private IQueryable<Product> GetApprovedProductsAsTracking()
        {
            return productsRepo.AllAsTracking()
            .Where(p => !p.IsDeleted && p.IsApproved);
        }

    }
}
