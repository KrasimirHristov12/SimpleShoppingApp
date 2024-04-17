using SimpleShoppingApp.Data.Enums;
using SimpleShoppingApp.Models.Addresses;
using System.ComponentModel.DataAnnotations;
using static SimpleShoppingApp.Data.Constants.GlobalConstants;

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

        [Display(Name = AddressIdDisplay)]
        public int AddressId { get; set; }

        [Display(Name = PaymentMethodDisplay)]
        public PaymentMethod PaymentMethod { get; set; }

        [Required]
        [Phone]
        [RegularExpression(PhoneRegex, ErrorMessage = PhoneNumberErrorMessage)]
        [Display(Name = PhoneNumberDisplay)]
        public string PhoneNumber { get; set; } = null!;

        public IEnumerable<AddressViewModel> Addresses { get; set; }

        public List<int> ProductIds { get; set; }

        public List<int> Quantities { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (ProductIds.Count == 0 || Quantities.Count == 0)
            {
                yield return new ValidationResult(NoProductsErrorMessage);
            }
        }
    }
}
