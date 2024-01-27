using Microsoft.EntityFrameworkCore;
using SimpleShoppingApp.Data.Enums;
using SimpleShoppingApp.Data.Models;
using SimpleShoppingApp.Data.Repository;
using SimpleShoppingApp.Models.Products;
using SimpleShoppingApp.Services.Categories;
using SimpleShoppingApp.Services.Images;
using System.Security.AccessControl;

namespace SimpleShoppingApp.Services.Products
{
    public class ProductsService : IProductsService
    {
        private readonly IRepository<Product> productsRepo;
        private readonly IImagesService imagesService;
        private readonly ICategoriesService categoriesService;

        public ProductsService(IRepository<Product> _productsRepo, 
            IImagesService _imagesService,
            ICategoriesService _categoriesService)
        {
            productsRepo = _productsRepo;
            imagesService = _imagesService;
            categoriesService = _categoriesService;
        }

        public async Task<int> AddAsync(AddProductInputModel model)
        {
            var product = new Product()
            {
                Name = model.Name,
                Description = model.Description,
                Price = (decimal)model.Price,
                Quantity = (int)model.Quantity,
                Rating = 0,
                CategoryId = model.CategoryId,
            };

            await productsRepo.AddAsync(product);
            await productsRepo.SaveChangesAsync();

            return product.Id;
        }

        public async Task<ProductViewModel?> GetAsync(int id)
        {
            var product = await productsRepo.AllAsNoTracking()
                .Where(p => p.Id == id)
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
                })
               .FirstOrDefaultAsync();

            if (product == null)
            {
                return null;
            }

            var images = await imagesService.GetAsync(id, ImageType.Product);

            product.Images = images;

            return product;
        }

        public async Task<IEnumerable<ListProductsViewModel>> GetRandomProductsAsync(int n)
        {
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
                }).ToListAsync();

            foreach (var product in products)
            {
                product.Image = await imagesService.GetFirstAsync(product.Id, ImageType.Product);
            }

            return products;
        }

        public async Task<IEnumerable<ListProductsViewModel>> GetByCategoryAsync(int categoryId, int elementsPerPage, int currentPage)
        {
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
                product.Image = await imagesService.GetFirstAsync(product.Id, ImageType.Product);
            }

            return products;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var productToDelete = await productsRepo.AllAsTracking().FirstOrDefaultAsync(p => p.Id == id);

            if (productToDelete == null)
            {
                return false;
            }

            productToDelete.IsDeleted = true;

            await productsRepo.SaveChangesAsync();

            return true;
        }
        
        public async Task<int> GetCountForCategoryAsync(int categoryId)
        {
            return await productsRepo.AllAsNoTracking()
                .Where(p => p.CategoryId == categoryId && !p.IsDeleted)
                .CountAsync();
        }

        public async Task<EditProductInputModel?> GetToEditAsync(int id)
        {
            var productToEdit = await productsRepo.AllAsNoTracking()
                .Where(p => p.Id == id)
                .Select(p => new EditProductInputModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    CategoryId = p.CategoryId,
                    Description = p.Description,
                    Quantity = p.Quantity,

                }).FirstOrDefaultAsync();

            if (productToEdit != null)
            {
                productToEdit.Categories = await categoriesService.GetAllAsync();
            }

            return productToEdit;
        }

        public async Task UpdateAsync(EditProductInputModel model)
        {
            var productToEdit = await productsRepo.AllAsTracking().FirstOrDefaultAsync(p => p.Id == model.Id);

            productToEdit.Name = model.Name;
            productToEdit.Price = (decimal)model.Price;
            productToEdit.Quantity = (int)model.Quantity;
            productToEdit.CategoryId = model.CategoryId;
            productToEdit.Description = model.Description;

            await productsRepo.SaveChangesAsync();
        }
    }
}
