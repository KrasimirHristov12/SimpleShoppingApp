namespace SimpleShoppingApp.Models.Carts
{
    public class UpdateQuantityJsonViewModel
    {

        public int UpdatedQuantity { get; set; }

        public decimal NewProductPrice { get; set; }

        public bool IsThereLessThanRequested { get; set; }

        public decimal NewTotalPrice { get; set; }
    }
}
