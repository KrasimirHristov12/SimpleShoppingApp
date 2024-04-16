using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SimpleShoppingApp.Extensions;
using SimpleShoppingApp.Services.Notifications;

namespace SimpleShoppingApp.Web.Hubs
{
    [Authorize]
    public class NotificationsHub : Hub
    {
        private readonly INotificationsService notificationsService;

        public NotificationsHub(INotificationsService _notificationsService)
        {
            notificationsService = _notificationsService;
        }
        public override Task OnConnectedAsync()
        {
            Clients.Caller.SendAsync("Connected");
            return base.OnConnectedAsync();
        }

        public async Task ReadAsync(int notificationId)
        {
            var userId = Context.User?.GetId();
            if (!string.IsNullOrWhiteSpace(userId))
            {
                await notificationsService.ReadAsync(userId, notificationId);
            }
        }
    }
}
