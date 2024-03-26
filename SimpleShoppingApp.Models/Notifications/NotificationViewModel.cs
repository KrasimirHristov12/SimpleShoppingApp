namespace SimpleShoppingApp.Models.Notifications
{
    public class NotificationViewModel
    {
        public int Id { get; set; }

        public string Text { get; set; } = null!;

        public string? Url { get; set; }

        public bool IsRead { get; set; }

    }
}
