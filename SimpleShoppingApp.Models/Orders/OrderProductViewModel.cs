namespace SimpleShoppingApp.Models.Orders
{
    public class OrderProductViewModel
    {
        public string Name { get; set; } = null!;

        public string Address { get; set; } = null!;

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public decimal TotalPrice => Quantity * Price;

        public int DeliveryDays { get; set; }
    }
}
