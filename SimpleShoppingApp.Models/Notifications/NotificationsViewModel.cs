namespace SimpleShoppingApp.Models.Notifications
{
    public class NotificationsViewModel
    {
        public NotificationsViewModel()
        {
            Notifications = new List<NotificationViewModel>();
        }

        public int NewNotificationsCount { get; set; }
        public IEnumerable<NotificationViewModel> Notifications { get; set; }
    }
}
