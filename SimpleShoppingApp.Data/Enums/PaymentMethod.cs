using System.ComponentModel.DataAnnotations;

namespace SimpleShoppingApp.Data.Enums
{
    public enum PaymentMethod
    {
        [Display(Name = "Credit / Debit Card")]
        Card = 1,
        [Display(Name = "PayPal")]
        PayPal = 2,
        [Display(Name = "Cash on delivery")]
        Cash = 3,
    }
}
