using System.ComponentModel.DataAnnotations;

namespace SimpleShoppingApp.Data.Enums
{
    public enum OrderStatus
    {
        [Display(Name = "Not Delivered")]
        NotDelivered,
        [Display(Name = "Delivered")]
        Delivered
    }
}
