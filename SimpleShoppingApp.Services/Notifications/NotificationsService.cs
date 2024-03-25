using SimpleShoppingApp.Data.Models;
using SimpleShoppingApp.Data.Repository;
using SimpleShoppingApp.Services.Users;

namespace SimpleShoppingApp.Services.Notifications
{
    public class NotificationsService : INotificationsService
    {
        private readonly IUsersService usersService;
        private readonly IRepository<Notification> notificationsRepo;

        public NotificationsService(IUsersService _usersService,
            IRepository<Notification> _notificationsRepo)
        {
            usersService = _usersService;
            notificationsRepo = _notificationsRepo;
        }
        public async Task<bool> AddAsync(string senderUserId,
            string receiverUserId,
            string notificationText,
            string? url = null)
        {
            if (!await usersService.DoesUserExistAsync(senderUserId))
            {
                return false;
            }

            if (!await usersService.DoesUserExistAsync(receiverUserId))
            {
                return false;
            }

            if (senderUserId == receiverUserId)
            {
                return false;
            }

            var notification = new Notification()
            {
                SenderUserId = senderUserId,
                ReceiverUserId = receiverUserId,
                Text = notificationText,
                Url = url,
            };

            await notificationsRepo.AddAsync(notification);
            await notificationsRepo.SaveChangesAsync();
            return true;


        }
    }
}
