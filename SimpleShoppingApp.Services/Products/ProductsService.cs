using Microsoft.EntityFrameworkCore;
using SimpleShoppingApp.Data.Enums;
using SimpleShoppingApp.Data.Models;
using SimpleShoppingApp.Data.Repository;
using SimpleShoppingApp.Models.Products;
using SimpleShoppingApp.Services.Categories;
using SimpleShoppingApp.Services.Images;
using SimpleShoppingApp.Services.Users;

namespace SimpleShoppingApp.Services.Products
{
    public class ProductsService : IProductsService
    {
        private readonly IRepository<Product> productsRepo;
        private readonly IImagesService imagesService;
        private readonly ICategoriesService categoriesService;
        private readonly IUsersService usersService;

        public ProductsService(
            IRepository<Product> _productsRepo,
            IImagesService _imagesService,
            ICategoriesService _categoriesService,
            IUsersService _usersService)
        {
            productsRepo = _productsRepo;
            imagesService = _imagesService;
            categoriesService = _categoriesService;
            usersService = _usersService;
        }

        public async Task<AddProductModel> AddAsync(AddProductInputModel model, string userId)
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

            var product = new Product()
            {
                Name = model.Name,
                Description = model.Description,
                Price = model.Price ?? 0,
                Quantity = model.Quantity ?? 0,
                Rating = 0,
                CategoryId = model.CategoryId,
                UserId = userId,
            };

            await productsRepo.AddAsync(product);
            await productsRepo.SaveChangesAsync();

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
                    Rating = p.Rating,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.Name,
                    UserName = p.User.UserName
                })
               .FirstOrDefaultAsync();

            if (product == null)
            {
                return null;
            }

            var images = await imagesService.GetAsync(id, ImageType.Product);

            product.Images = images;

            if (userId == null)
            {
                product.BelongsToCurrentUser = false;
            }
            else
            {
                product.BelongsToCurrentUser = await BelognsToUserAsync(id, userId);
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
                var image = await imagesService.GetFirstAsync(product.Id, ImageType.Product);
                if (image != null)
                {
                    product.Image = image;
                }
                
            }

            return products;
        }

        public async Task<IEnumerable<ListProductsViewModel>?> GetByCategoryAsync(int categoryId, int elementsPerPage, int currentPage)
        {
            if (categoryId <= 0 || elementsPerPage <= 0 || currentPage <= 0)
            {
                return null;
            }

            if (!await categoriesService.DoesCategoryExist(categoryId))
            {
                return null;
            }

            var products = await productsRepo.AllAsNoTracking()
                .Where(p => p.CategoryId == categoryId && !p.IsDeleted)
                .Skip((currentPage - 1) * elementsPerPage)
                .Take(elementsPerPage)
                .Select(p => new ListProductsViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Rating = p.Rating,

                }).ToListAsync();

            foreach (var product in products)
            {
                var image = await imagesService.GetFirstAsync(product.Id, ImageType.Product);
                if (image != null)
                {
                    product.Image = image;
                }
            }

            return products;
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
                var image = await imagesService.GetFirstAsync(product.Id, ImageType.Product);
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
    }
}
