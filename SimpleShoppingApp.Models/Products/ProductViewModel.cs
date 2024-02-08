using SimpleShoppingApp.Models.Images;

namespace SimpleShoppingApp.Models.Products
{
    public class ProductViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public double Rating { get; set; }
        public int CategoryId { get; set; }

        public string CategoryName { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;

        public bool BelongsToCurrentUser { get; set; }

        public IEnumerable<ImageViewModel> Images { get; set; } = new List<ImageViewModel>();
    }
}
