namespace SimpleShoppingApp.Models.Products
{
    public class ProductReviewViewModel
    {
        public int Rating { get; set; }
        public string UserName { get; set; } = null!;
        public string? Text { get; set; }
        public bool IsMine { get; set; }
    }
}
