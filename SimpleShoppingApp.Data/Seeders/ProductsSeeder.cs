using SimpleShoppingApp.Data.Models;
using SimpleShoppingApp.Data.Repository;

namespace SimpleShoppingApp.Data.Seeders
{
    public class ProductsSeeder : ISeeder
    {
        private readonly IRepository<Product> productsRepo;

        public ProductsSeeder(IRepository<Product> _productsRepo)
        {
            productsRepo = _productsRepo;
        }
        public async Task SeedAsync()
        {
            if (productsRepo.AllAsNoTracking().Count() == 0)
            {
                var random = new Random();

                for (int i = 1; i <= 50; i++)
                {
                    var product = new Product
                    {
                        Name = $"Test Product {i}",
                        Description = $"Test Product Description {i}",
                        Price = random.Next(5000),
                        Quantity = random.Next(5000),
                        UserId = "e6cc76b9-19a0-4753-bcac-cea4a41df3b3",
                        CategoryId = random.Next(1,4),
                    };
                    var image = new Image
                    {
                        ImageUrl = "https://static.wikia.nocookie.net/nicos-nextbots/images/8/81/Hopper.png/revision/latest?cb=20221212182747",
                    };

                    product.Images.Add(image);

                    await productsRepo.AddAsync(product);
                }

                await productsRepo.SaveChangesAsync();
            }

        }
    }
}
