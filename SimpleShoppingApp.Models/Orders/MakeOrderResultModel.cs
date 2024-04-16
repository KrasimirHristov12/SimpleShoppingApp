using SimpleShoppingApp.Data.Enums;
namespace SimpleShoppingApp.Models.Orders
{
    public class MakeOrderResultModel
    {
        public MakeOrderResultModel()
        {
            ProductsIds = new List<int>();
        }
        public MakeOrderResult Result { get; set; }

        public List<int> ProductsIds { get; set; }

        public List<int> Quantities { get; set; }

        public string? ErrorMessage { get; set; }
    }
}
