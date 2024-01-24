using Microsoft.EntityFrameworkCore;
using SimpleShoppingApp.Data.Enums;
using SimpleShoppingApp.Data.Models;
using SimpleShoppingApp.Data.Repository;
using SimpleShoppingApp.Models.Products;
using SimpleShoppingApp.Services.Images;

namespace SimpleShoppingApp.Services.Products
{
    public class ProductsService : IProductsService
    {
        private readonly IRepository<Product> productsRepo;
        private readonly IImagesService imagesService;

        public ProductsService(IRepository<Product> _productsRepo, IImagesService _imagesService)
        {
            productsRepo = _productsRepo;
            imagesService = _imagesService;
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

        public async Task<IEnumerable<ProductOnIndexViewModel>> GetRandomProductsAsync(int n)
        {
            var products = await productsRepo.AllAsNoTracking()
                .OrderBy(p => Guid.NewGuid())
                .Take(n)
                .Select(p => new ProductOnIndexViewModel
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
    }
}
