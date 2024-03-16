namespace SimpleShoppingApp.Models.Products
{
    public class ProductsPerPageModel
    {
        public ProductsPerPageModel()
        {
            Products = new List<ListProductsViewModel>();
        }

        public int TotalProducts { get; set; }

        public int TotalPages { get; set; }

        public IEnumerable<ListProductsViewModel> Products { get; set; }
    }
}
