using System.ComponentModel.DataAnnotations;
using static SimpleShoppingApp.Data.Constants.GlobalConstants;

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
        [MaxLength(AddressNameMaxLength)]
        public string Name { get; set; } = null!;

        [Required]
        public string UserId { get; set; } = null!;

        [Required]
        public ApplicationUser User { get; set; } = null!;

        public bool IsDeleted { get; set; }

        public ICollection<Order> Orders { get; set; }
    }
}
