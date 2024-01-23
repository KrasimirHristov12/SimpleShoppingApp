using SimpleShoppingApp.Models.Products;

namespace SimpleShoppingApp.Services.Products
{
    public interface IProductsService
    {
        Task<int> AddAsync(AddProductInputModel model);

        Task<ProductViewModel?> GetAsync(int id);
    }
}
