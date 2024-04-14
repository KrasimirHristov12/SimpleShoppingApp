using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using static SimpleShoppingApp.Data.Constants.GlobalConstants;

namespace SimpleShoppingApp.Data.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            Products = new HashSet<Product>();
            Orders = new HashSet<Order>();
            Addresses = new HashSet<ShippingAddress>();
            UsersRating = new HashSet<UsersRating>();
            SendedNotifications = new HashSet<Notification>();
            ReceivedNotifications = new HashSet<Notification>();
        }

        [MaxLength(UserMaxLength)]
        public string? FirstName { get; set; }

        [MaxLength(UserMaxLength)]   
        public string? LastName { get; set; }

        public ShoppingCart Cart { get; set; } = null!;

        public int CartId { get; set; }

        public ICollection<Product> Products { get; set; }

        public ICollection<Order> Orders { get; set; }

        public ICollection<ShippingAddress> Addresses { get; set; }

        public ICollection<UsersRating> UsersRating { get; set; }

        public ICollection<Notification> SendedNotifications { get; set; }

        public ICollection<Notification> ReceivedNotifications { get; set; }

    }
}
