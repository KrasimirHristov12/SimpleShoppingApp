using System.ComponentModel.DataAnnotations;
using static SimpleShoppingApp.Data.Constants.GlobalConstants;

namespace SimpleShoppingApp.Data.Models
{
    public class UsersRating
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = null!;

        [Required]
        public ApplicationUser User { get; set; } = null!;

        public int ProductId { get; set; }

        [Required]
        public Product Product { get; set; } = null!;

        public int Rating { get; set; }


        [MaxLength(ReviewTextMaxLength)]
        public string? Text { get; set; }
    }
}
