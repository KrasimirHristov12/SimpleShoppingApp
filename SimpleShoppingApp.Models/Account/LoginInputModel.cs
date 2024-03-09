using System.ComponentModel.DataAnnotations;

namespace SimpleShoppingApp.Models.Account
{
    public class LoginInputModel
    {
        [Required]
        [EmailAddress]
        [StringLength(100, MinimumLength = 6)]
        public string Email { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
    }
}
