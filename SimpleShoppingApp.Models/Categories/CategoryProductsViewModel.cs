using SimpleShoppingApp.Models.Products;

namespace SimpleShoppingApp.Models.Categories
{
    public class CategoryProductsViewModel
    {
        public string? CategoryName { get; set; }

        public int TotalProductsCount { get; set; }

        public IEnumerable<ListProductsViewModel>? ProductsPerPage { get; set; }


    }
}
