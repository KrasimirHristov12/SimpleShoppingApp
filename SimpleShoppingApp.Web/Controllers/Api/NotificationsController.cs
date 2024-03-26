using Microsoft.AspNetCore.Mvc;
using SimpleShoppingApp.Extensions;
using SimpleShoppingApp.Services.Notifications;

namespace SimpleShoppingApp.Web.Controllers.Api
{
    public class NotificationsController : BaseApiController
    {
        private readonly INotificationsService notificationsService;

        public NotificationsController(INotificationsService _notificationsService)
        {
            notificationsService = _notificationsService;
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<ActionResult<bool>> Read([FromForm]int notificationId)
        {
            var userId = User.GetId();
            var result = await notificationsService.ReadAsync(userId, notificationId);
            return Ok(result);
        }
    }
}
