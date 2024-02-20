using SimpleShoppingApp.Models.Orders;

namespace SimpleShoppingApp.Models.Carts
{
    public class CartViewModel
    {
        public IEnumerable<CartProductsViewModel> CartProducts { get; set; } = null!;

        public MakeOrderInputModel Input { get; set; } = null!;

        public decimal TotalPrice => CartProducts.Sum(cp => cp.ProductTotalPrice);

        public int ProductsCount => CartProducts.Count();
    }
}
