using System.ComponentModel.DataAnnotations;
using static SimpleShoppingApp.Data.Constants.GlobalConstants;

namespace SimpleShoppingApp.Models.Addresses
{
    public class AddAddressInputModel
    {
        [Required]
        [MaxLength(AddressNameMaxLength)]
        [Display(Name = AddressDisplay)]
        public string Name { get; set; } = null!;
    }
}
