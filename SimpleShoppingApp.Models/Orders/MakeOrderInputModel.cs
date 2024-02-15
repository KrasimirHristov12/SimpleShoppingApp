namespace SimpleShoppingApp.Models.Orders
{
    public class MakeOrderInputModel
    {
        public MakeOrderInputModel()
        {
            ProductIds = new List<int>();
            Quantities = new List<int>();
        }
        public List<int> ProductIds { get; set; }

        public List<int> Quantities { get; set; }
    }
}
