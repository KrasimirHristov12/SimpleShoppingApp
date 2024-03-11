using System.ComponentModel.DataAnnotations;

namespace SimpleShoppingApp.Models.Account
{
    public class ForgotPasswordInputModel
    {
        [Required]
        [StringLength(100, MinimumLength = 6)]
        [EmailAddress]
        public string Email { get; set; } = null!;
    }
}
