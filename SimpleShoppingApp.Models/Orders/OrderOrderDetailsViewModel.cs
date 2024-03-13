using SimpleShoppingApp.Data.Enums;

namespace SimpleShoppingApp.Models.Orders
{
    public class OrderOrderDetailsViewModel
    {
        public int Id { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime DeliveryDate { get; set; }

        public string OrderStatus { get; set; } = null!;

        public int DeliveryDays { get; set; }

        public decimal TotalPrice { get; set; }

        public string Address { get; set; } = null!;

        public string PaymentMethod { get; set; } = null!;

        public PaymentMethod PaymentMethodEnum { get; set; }
    }
}
