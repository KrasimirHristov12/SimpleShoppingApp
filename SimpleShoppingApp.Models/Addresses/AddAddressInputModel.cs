using System.ComponentModel.DataAnnotations;

namespace SimpleShoppingApp.Models.Addresses
{
    public class AddAddressInputModel
    {
        [Required]
        [MaxLength(100)]
        [Display(Name = "Address")]
        public string Name { get; set; } = null!;
    }
}
