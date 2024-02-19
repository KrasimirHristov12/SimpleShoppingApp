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
        }
        public int Id { get; set; }

        [Required]
        public ApplicationUser User { get; set; } = null!;

        [Required]
        public string UserId { get; set; } = null!;

        public ICollection<OrdersProducts> OrdersProducts { get; set; }

        public DateTime CreatedOn { get; set; }

        public OrderStatus OrderStatus { get; set; }

        public bool IsDeleted { get; set; }
    }
}
