using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace SimpleShoppingApp.Data.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            Products = new HashSet<Product>();
            Orders = new HashSet<Order>();
            Addresses = new HashSet<ShippingAddress>();
        }

        [MaxLength(50)]
        public string? FirstName { get; set; }

        [MaxLength(50)]   
        public string? LastName { get; set; }

        public ShoppingCart Cart { get; set; } = null!;

        public int CartId { get; set; }

        public ICollection<Product> Products { get; set; }

        public ICollection<Order> Orders { get; set; }

        public ICollection<ShippingAddress> Addresses { get; set; }


    }
}
