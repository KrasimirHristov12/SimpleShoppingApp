namespace SimpleShoppingApp.Services.Notifications
{
    public interface INotificationsService
    {
        Task<bool> AddAsync(string senderUserId,
            string receiverUserId,
            string notificationText,
            string? url = null);
    }
}
