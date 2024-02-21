using SimpleShoppingApp.Data.Enums;
using SimpleShoppingApp.Models.Addresses;
using System.ComponentModel.DataAnnotations;

namespace SimpleShoppingApp.Models.Orders
{
    public class MakeOrderInputModel : IValidatableObject
    {
        public MakeOrderInputModel()
        {
            ProductIds = new List<int>();
            Quantities = new List<int>();
            Addresses = new List<AddressViewModel>();
        }

        [Display(Name = "Shipping Address")]
        public int AddressId { get; set; }

        [Display(Name = "Payment Method")]
        public PaymentMethod PaymentMethod { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; } = null!;

        public IEnumerable<AddressViewModel> Addresses { get; set; }

        public List<int> ProductIds { get; set; }

        public List<int> Quantities { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (ProductIds.Count == 0 || Quantities.Count == 0)
            {
                yield return new ValidationResult("You should add at least one product to proceed with the order");
            }
        }
    }
}
