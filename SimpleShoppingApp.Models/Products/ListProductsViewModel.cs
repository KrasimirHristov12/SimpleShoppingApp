using SimpleShoppingApp.Models.Images;

namespace SimpleShoppingApp.Models.Products
{
    public class ListProductsViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public decimal Price { get; set; }

        public double Rating { get; set; }

        public bool HasDiscount { get; set; }

        public decimal? NewPrice { get; set; }

        public int? DiscountPercentage => NewPrice == null ? null : 100 - (int)(NewPrice / Price * 100);

        public ImageViewModel Image { get; set; } = null!;
    }
}
