using System.ComponentModel.DataAnnotations;
using static SimpleShoppingApp.Data.Constants.GlobalConstants;

namespace SimpleShoppingApp.Models.Addresses
{
    public class AddAddressInputModel
    {
        [Required]
        [StringLength(AddressNameMaxLength, MinimumLength = AddressNameMinLength)]
        [Display(Name = AddressDisplay)]
        public string Name { get; set; } = null!;
    }
}
