using SimpleShoppingApp.Data.Enums;

namespace SimpleShoppingApp.Models.Products
{
    public class EditProductModel
    {
        public AddUpdateDeleteResult Result { get; set; }

        public EditProductInputModel? Model { get; set; }
    }
}
