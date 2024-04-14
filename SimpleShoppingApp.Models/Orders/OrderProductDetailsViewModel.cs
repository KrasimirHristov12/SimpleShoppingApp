using SimpleShoppingApp.Data.Enums;
using SimpleShoppingApp.Models.Images;

namespace SimpleShoppingApp.Models.Orders
{
    public class OrderProductDetailsViewModel
    {
        public OrderProductDetailsViewModel()
        {
            Image = new ImageViewModel();
        }

        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public ImageViewModel? Image { get; set; }
    }
}
