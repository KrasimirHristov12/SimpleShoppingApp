using SimpleShoppingApp.Data.Enums;

namespace SimpleShoppingApp.Models.Products
{
    public class ProductsFilterModel
    {
        public ProductsFilterModel()
        {
            Prices = new List<PriceFilter>();
            Ratings = new List<RatingFilter>();
            Page = 1;
        }
        public IEnumerable<PriceFilter> Prices { get; set; }

        public IEnumerable<RatingFilter> Ratings { get; set; }

        public int Page { get; set; }

        public int ProductsPerPage { get; set; }

        public int CategoryId { get; set; }

    }
}
