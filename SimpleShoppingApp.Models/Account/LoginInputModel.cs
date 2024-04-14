using System.ComponentModel.DataAnnotations;
using static SimpleShoppingApp.Data.Constants.GlobalConstants;

namespace SimpleShoppingApp.Models.Account
{
    public class LoginInputModel
    {
        [Required]
        [EmailAddress]
        [StringLength(EmailMaxLength, MinimumLength = EmailMinLength)]
        public string Email { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [Display(Name = RememberMeDisplay)]
        public bool RememberMe { get; set; }
    }
}
