using System.ComponentModel.DataAnnotations;

namespace SimpleShoppingApp.Models.Users
{
    public class EditPhoneNumberInputModel
    {
        [Required]
        [Phone]
        public string PhoneNumber { get; set; } = null!;
    }
}
