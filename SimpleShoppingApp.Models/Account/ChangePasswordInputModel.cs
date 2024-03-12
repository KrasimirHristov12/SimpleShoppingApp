using System.ComponentModel.DataAnnotations;

namespace SimpleShoppingApp.Models.Account
{
    public class ChangePasswordInputModel
    {
        [Required]
        [MinLength(6)]
        [DataType(DataType.Password)]
        [Display(Name = "Current Password")]
        public string CurrentPassword { get; set; } = null!;

        [Required]
        [MinLength(6)]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; } = null!;

        [Required]
        [MinLength(6)]
        [DataType(DataType.Password)]
        [Compare(nameof(NewPassword))]
        [Display(Name = "Confirm new Password")]
        public string ConfirmNewPassword { get; set; } = null!;
    }
}
