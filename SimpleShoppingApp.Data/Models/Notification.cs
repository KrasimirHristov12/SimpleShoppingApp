namespace SimpleShoppingApp.Data.Models
{
    public class Notification
    {
        public int Id { get; set; }

        public string Text { get; set; } = null!;

        public string SenderUserId { get; set; } = null!;

        public string ReceiverUserId { get; set; } = null!;

        public ApplicationUser SenderUser { get; set; } = null!;

        public ApplicationUser ReceiverUser { get; set; } = null!;

        public string? Url { get; set; }

        public bool IsDeleted { get; set; }
    }
}
