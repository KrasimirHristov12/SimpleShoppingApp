using SimpleShoppingApp.Models.Images;

namespace SimpleShoppingApp.Models.Products
{
    public class ProductViewModel
    {
        public ProductViewModel()
        {
            Images = new List<ImageViewModel>();
        }
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string Description { get; set; } = null!;

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public double Rating { get; set; }

        public int CategoryId { get; set; }

        public string CategoryName { get; set; } = null!;

        public string UserName { get; set; } = null!;

        public bool BelongsToCurrentUser { get; set; }

        public IEnumerable<ImageViewModel> Images { get; set; }
    }
}
