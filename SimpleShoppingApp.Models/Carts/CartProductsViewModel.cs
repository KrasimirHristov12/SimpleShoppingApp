using SimpleShoppingApp.Models.Images;

namespace SimpleShoppingApp.Models.Carts
{
    public class CartProductsViewModel
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; } = null!;

        public int ProductQuantity { get; set; }

        public decimal ProductPrice { get; set; }

        public ImageViewModel Image { get; set; } = null!;

        public decimal ProductTotalPrice => ProductQuantity * ProductPrice;
    }
}
