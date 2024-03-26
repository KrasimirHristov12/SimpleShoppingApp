using Microsoft.AspNetCore.Mvc;
using SimpleShoppingApp.Services.Notifications;
using System.Security.Claims;

namespace SimpleShoppingApp.Web.ViewComponents
{
    public class NotificationsViewComponent : ViewComponent
    {
        private readonly INotificationsService notificationsService;

        public NotificationsViewComponent(INotificationsService _notificationsService)
        {
            notificationsService = _notificationsService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userId = UserClaimsPrincipal.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var notifications = await notificationsService.GetNotificationsAsync(1, 10, userId);
            return View(notifications);
        }
    }
}
