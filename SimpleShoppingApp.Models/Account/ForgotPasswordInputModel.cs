using System.ComponentModel.DataAnnotations;
using static SimpleShoppingApp.Data.Constants.GlobalConstants;

namespace SimpleShoppingApp.Models.Account
{
    public class ForgotPasswordInputModel
    {
        [Required]
        [StringLength(EmailMaxLength, MinimumLength = EmailMinLength)]
        [EmailAddress]
        public string Email { get; set; } = null!;
    }
}
