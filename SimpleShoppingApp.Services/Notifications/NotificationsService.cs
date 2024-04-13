using Microsoft.EntityFrameworkCore;
using SimpleShoppingApp.Data.Models;
using SimpleShoppingApp.Data.Repository;
using SimpleShoppingApp.Models.Notifications;
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

        public async Task<NotificationsViewModel> GetNotificationsAsync(int numberOfElements, string userId)
        {
            var newNotificationsCount = await GetNewNotificationsCountAsync(userId);
            var notificationsQuery = GetNotDeleted()
                .Where(n => n.ReceiverUserId == userId)
                .Select(n => new NotificationViewModel
                {
                    Id = n.Id,
                    Text = n.Text,
                    Url = n.Url,
                    IsRead = n.IsRead,
                })
                .OrderByDescending(n => n.Id);


            var notReadNotifications = await notificationsQuery.Where(n => !n.IsRead)
                .Take(numberOfElements)
                .ToListAsync();

            int notReadCount = notReadNotifications.Count;


            var readNotifications = await notificationsQuery.Where(n => n.IsRead)
                .Take(numberOfElements - notReadCount)
                .ToListAsync();

            var notifications = notReadNotifications.Concat(readNotifications);


            return new NotificationsViewModel
            {
                NewNotificationsCount = newNotificationsCount,
                Notifications = notifications,
            };
        }

        public async Task<int> GetNewNotificationsCountAsync(string userId)
        {
            return await GetNotDeleted()
                .Where(n => !n.IsRead && n.ReceiverUserId == userId)
                .CountAsync();
        }

        public async Task<bool> ReadAsync(string userId, int notificationId)
        {
            var notification = await GetNotDeletedAsTracking()
                .Where(n => n.ReceiverUserId == userId && n.Id == notificationId && !n.IsRead)
                .FirstOrDefaultAsync();

            if (notification == null)
            {
                return false;
            }

            notification.IsRead = true;


            await notificationsRepo.SaveChangesAsync();
            return true;
        }

        private IQueryable<Notification> GetNotDeleted()
        {
            return notificationsRepo.AllAsNoTracking()
                .Where(n => !n.IsDeleted);
        }

        public IQueryable<Notification> GetNotDeletedAsTracking()
        {
            return notificationsRepo.AllAsTracking()
                .Where(n => !n.IsDeleted);
        }
    }
}
