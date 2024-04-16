using SimpleShoppingApp.Data.Enums;
namespace SimpleShoppingApp.Models.Products
{
    public class AddProductModel
    {
        public AddUpdateDeleteResult Result { get; set; }

        public int? ProductId { get; set; }

        public bool IsApproved { get; set; }
    }
}
