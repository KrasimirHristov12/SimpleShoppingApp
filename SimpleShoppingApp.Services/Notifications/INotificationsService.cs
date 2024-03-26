using SimpleShoppingApp.Models.Notifications;

namespace SimpleShoppingApp.Services.Notifications
{
    public interface INotificationsService
    {
        Task<bool> AddAsync(string senderUserId,
            string receiverUserId,
            string notificationText,
            string? url = null);

        Task<NotificationsViewModel> GetNotificationsAsync(int page, int numberOfElements, string userId);

        Task<int> GetNewNotificationsCountAsync(string userId);

        Task<bool> ReadAsync(string userId, int notificationId);
    }
}
