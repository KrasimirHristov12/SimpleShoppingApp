using System.ComponentModel.DataAnnotations;

namespace SimpleShoppingApp.Data.Models
{
    public class ShippingAddress
    {
        public ShippingAddress()
        {
            Orders = new HashSet<Order>();
        }
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        [Required]
        public string UserId { get; set; } = null!;

        [Required]
        public ApplicationUser User { get; set; } = null!;

        public bool IsDeleted { get; set; }

        public ICollection<Order> Orders { get; set; }
    }
}
