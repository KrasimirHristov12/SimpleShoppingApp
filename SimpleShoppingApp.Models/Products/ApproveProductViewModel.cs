using SimpleShoppingApp.Models.Images;

namespace SimpleShoppingApp.Models.Products
{
    public class ApproveProductViewModel
    {
        public ApproveProductViewModel()
        {
            Images = new List<ImageViewModel>();
        }
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string Description { get; set; } = null!;

        public decimal Price { get; set; }

        public string CategoryName { get; set; } = null!;

        public IEnumerable<ImageViewModel> Images { get; set; }

    }
}
