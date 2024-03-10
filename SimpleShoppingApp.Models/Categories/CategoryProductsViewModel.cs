using SimpleShoppingApp.Models.Products;

namespace SimpleShoppingApp.Models.Categories
{
    public class CategoryProductsViewModel
    {
        public CategoryProductsViewModel()
        {
            ProductsPerPage = new List<ListProductsViewModel>();
        }
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }

        public int TotalProductsCount { get; set; }

        public IEnumerable<ListProductsViewModel> ProductsPerPage { get; set; }

        public int ProductsPerPageCount => ProductsPerPage.Count();

        public int TotalPages => (int)Math.Ceiling((decimal)TotalProductsCount / ProductsPerPageCount);

        public int CurrentPage { get; set; }


    }
}
