using System.ComponentModel.DataAnnotations;
using static SimpleShoppingApp.Data.Constants.GlobalConstants;

namespace SimpleShoppingApp.Models.Products
{
    public class AddUpdateProductRatingJsonModel
    {
        public int ProductId { get; set; }

        public int Rating { get; set; }

        [StringLength(ReviewTextMaxLength, MinimumLength = ReviewTextMinLength)]
        public string? Text { get; set; }
    }
}
