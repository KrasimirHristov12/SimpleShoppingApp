using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Build.ObjectModelRemoting;
using SimpleShoppingApp.Data.Models;
using SimpleShoppingApp.Extensions;
using SimpleShoppingApp.Services.Notifications;
using SimpleShoppingApp.Services.Products;
using SimpleShoppingApp.Web.Hubs;

namespace SimpleShoppingApp.Web.Areas.Administration.Controllers
{
    public class ProductsController : AdministrationBaseController
    {
        private readonly IProductsService productsService;
        private readonly INotificationsService notificationsService;
        private readonly IHubContext<NotificationsHub> notificationsHub;

        public ProductsController(IProductsService _productsService, 
            INotificationsService _notificationsService,
            IHubContext<NotificationsHub> _notificationsHub)
        {
            productsService = _productsService;
            notificationsService = _notificationsService;
            notificationsHub = _notificationsHub;
        }

        [HttpPost]
        public async Task<IActionResult> Approve(int id)
        {
            var adminUserId = User.GetId();
            var approveResult = await productsService.ApproveAsync(id, adminUserId);

            if (!approveResult)
            {
                return NotFound();
            }

            var ownerUserId = await productsService.GetOwnerIdAsync(id);

            string notificationText = "A product you have created has been approved by the administrator";

            string productLink = $"/Products/Index/{id}";

            if (!string.IsNullOrWhiteSpace(ownerUserId))
            {
                var notificationId = await notificationsService.AddAsync(adminUserId, ownerUserId, notificationText, productLink);
                if (notificationId > 0)
                {
                    await notificationsHub.Clients.User(ownerUserId).SendAsync("ReceiveNotification", notificationText, productLink, notificationId);

                }

            }

            TempData["ProductApproved"] = "You have successfully approved this product.";

            return RedirectToAction("Index", "Products", new { id, area = "" });
        }

        [HttpPost]
        public async Task<IActionResult> UnApprove(int id)
        {
            var adminUserId = User.GetId();
            var ownerUserId = await productsService.GetOwnerIdAsync(id);
            var approveResult = await productsService.UnApproveAsync(id, adminUserId);
            if (!approveResult)
            {
                return NotFound();
            }

            if (!string.IsNullOrWhiteSpace(ownerUserId))
            {
                var notificationText = "A product you have created has not been approved by the administrator";
                var notificationId = await notificationsService.AddAsync(adminUserId, ownerUserId, notificationText);
                if (notificationId > 0)
                {
                    await notificationsHub.Clients.User(ownerUserId).SendAsync("ReceiveNotification", notificationText, "#", notificationId);

                }

            }


            TempData["ProductUnApproved"] = "You have not approved the product.";

            return RedirectToAction("Index", "Home", new { area = "" });
        }
    }
}
