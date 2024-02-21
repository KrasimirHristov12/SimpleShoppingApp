using SimpleShoppingApp.Data.Enums;

namespace SimpleShoppingApp.Models.Carts
{
    public class UpdateQuantityModel
    {
        public AddUpdateDeleteResult Result { get; set; }

        public UpdateQuantityJsonViewModel? Model { get; set; }
    }
}
