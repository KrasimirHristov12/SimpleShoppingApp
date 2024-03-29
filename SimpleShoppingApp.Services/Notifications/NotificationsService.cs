﻿using Microsoft.EntityFrameworkCore;
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

        public async Task<NotificationsViewModel> GetNotificationsAsync(int page, int numberOfElements, string userId)
        {
            var newNotificationsCount = await GetNewNotificationsCountAsync(userId);
            var notifications = await notificationsRepo.AllAsNoTracking()
                .Where(n => n.ReceiverUserId == userId && !n.IsDeleted)
                .Select(n => new NotificationViewModel
                {
                    Id = n.Id,
                    Text = n.Text,
                    Url = n.Url,
                    IsRead = n.IsRead,
                })
                .Skip((page - 1) * numberOfElements)
                .Take(numberOfElements)
                .OrderByDescending(n => n.Id)
                .ToListAsync();

            return new NotificationsViewModel
            {
                NewNotificationsCount = newNotificationsCount,
                Notifications = notifications,
            };
        }

        public async Task<int> GetNewNotificationsCountAsync(string userId)
        {
            return await notificationsRepo.AllAsNoTracking()
                .Where(n => !n.IsRead && !n.IsDeleted && n.ReceiverUserId == userId)
                .CountAsync();
        }

        public async Task<bool> ReadAsync(string userId, int notificationId)
        {
            var notification = await notificationsRepo
                .AllAsTracking()
                .Where(n => n.ReceiverUserId == userId && n.Id == notificationId && !n.IsRead && !n.IsDeleted)
                .FirstOrDefaultAsync();

            if (notification == null)
            {
                return false;
            }

            notification.IsRead = true;


            await notificationsRepo.SaveChangesAsync();
            return true;
        }
    }
}
