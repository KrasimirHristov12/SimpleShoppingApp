using SimpleShoppingApp.Data.Enums;

namespace SimpleShoppingApp.Models.Products
{
    public class ProductRatingViewModel
    {
        public AddUpdateDeleteResult Result { get; set; }
        public double? AvgRating { get; set; }
    }
}
