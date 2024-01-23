using SimpleShoppingApp.Data.Models;
using SimpleShoppingApp.Data.Repository;
using SimpleShoppingApp.Models.Products;

namespace SimpleShoppingApp.Services.Products
{
    public class ProductsService : IProductsService
    {
        private readonly IRepository<Product> productsRepo;

        public ProductsService(IRepository<Product> _productsRepo)
        {
            productsRepo = _productsRepo;
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
    }
}
