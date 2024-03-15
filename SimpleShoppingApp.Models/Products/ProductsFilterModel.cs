using SimpleShoppingApp.Data.Enums;

namespace SimpleShoppingApp.Models.Products
{
    public class ProductsFilterModel
    {
        public ProductsFilterModel()
        {
            Prices = new List<PriceFilter>();
            Ratings = new List<RatingFilter>();
        }
        public int Category { get; set; }
        public IEnumerable<PriceFilter> Prices { get; set; }

        public IEnumerable<RatingFilter> Ratings { get; set; }
    }
}
