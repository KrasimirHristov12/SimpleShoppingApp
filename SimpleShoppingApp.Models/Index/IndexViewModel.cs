using SimpleShoppingApp.Models.Products;

namespace SimpleShoppingApp.Models.Index
{
    public class IndexViewModel
    {
        public IndexViewModel()
        {
            RandomProducts = new List<ListProductsViewModel>();
        }
        public IEnumerable<ListProductsViewModel> RandomProducts { get; set; }

        public int TotalProductsCount { get; set; }

        public int TotalCategoriesCount { get; set; }
    }
}
