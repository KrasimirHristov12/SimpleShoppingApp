using SimpleShoppingApp.Models.Images;

namespace SimpleShoppingApp.Models.Products
{
    public class ListProductsViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public decimal Price { get; set; }

        public double Rating { get; set; }

        public ImageViewModel Image { get; set; } = null!;
    }
}
