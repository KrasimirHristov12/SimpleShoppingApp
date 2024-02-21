using SimpleShoppingApp.Data.Enums;

namespace SimpleShoppingApp.Models.Carts
{
    public class RemoveProductFromCartModel
    {
        public AddUpdateDeleteResult Result { get; set; }

        public CartJsonViewModel? Model { get; set; }
    }
}
