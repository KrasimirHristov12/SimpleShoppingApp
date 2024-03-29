using SimpleShoppingApp.Data.Enums;
namespace SimpleShoppingApp.Models.Orders
{
    public class MakeOrderResultModel
    {
        public MakeOrderResult Result { get; set; }

        public string? ErrorMessage { get; set; }
    }
}
