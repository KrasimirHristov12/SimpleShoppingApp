using SimpleShoppingApp.Models.Products;

namespace SimpleShoppingApp.Services.Products
{
    public interface IProductsService
    {
        Task<int> AddAsync(AddProductInputModel model);

        Task<ProductViewModel?> GetAsync(int id);

        Task<IEnumerable<ListProductsViewModel>> GetRandomProductsAsync(int n);

        Task<IEnumerable<ListProductsViewModel>> GetByCategoryAsync(int categoryId, int elementsPerPage, int currentPage);
    }
}
