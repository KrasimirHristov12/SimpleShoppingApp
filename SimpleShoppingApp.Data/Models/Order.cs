using SimpleShoppingApp.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace SimpleShoppingApp.Data.Models
{
    public class Order
    {
        public Order()
        {
            OrdersProducts = new HashSet<OrdersProducts>();
            CreatedOn = DateTime.UtcNow;
            Random rand = new Random();
            int deliveryDays = rand.Next(2, 4);
            DeliveryDate = DateTime.UtcNow.AddDays(deliveryDays);
        }
        public int Id { get; set; }

        [Required]
        public ApplicationUser User { get; set; } = null!;

        [Required]
        public string UserId { get; set; } = null!;

        public ICollection<OrdersProducts> OrdersProducts { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime DeliveryDate { get; set; }

        [Required]
        [MaxLength(10)]
        public string PhoneNumber { get; set; } = null!;

        public int AddressId { get; set; }

        [Required]
        public ShippingAddress Address { get; set; } = null!;

        public bool IsDeleted { get; set; }
    }
}
