namespace SimpleShoppingApp.Models.Orders
{
    public class OrderDetailsViewModel
    {
        public OrderDetailsViewModel()
        {
            Products = new List<OrderProductDetailsViewModel>();
        }
        public OrderOrderDetailsViewModel Order { get; set; } = null!;

        public IEnumerable<OrderProductDetailsViewModel> Products { get; set; }
    }
}
