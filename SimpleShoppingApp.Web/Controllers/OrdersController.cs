using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Build.ObjectModelRemoting;
using SimpleShoppingApp.Data.Enums;
using SimpleShoppingApp.Extensions;
using SimpleShoppingApp.Models.Orders;
using SimpleShoppingApp.Services.Addresses;
using SimpleShoppingApp.Services.Carts;
using SimpleShoppingApp.Services.Notifications;
using SimpleShoppingApp.Services.Orders;
using SimpleShoppingApp.Services.Products;
using SimpleShoppingApp.Services.Users;
using SimpleShoppingApp.Web.Hubs;

namespace SimpleShoppingApp.Web.Controllers
{
    public class OrdersController : BaseController
    {
        private readonly IOrdersService ordersService;
        private readonly ICartsService cartsService;
        private readonly IAddressesService addressesService;
        private readonly IProductsService productsService;
        private readonly IUsersService usersService;
        private readonly INotificationsService notificationsService;
        private readonly IHubContext<NotificationsHub> notificationsHub;

        public OrdersController(IOrdersService _ordersService,
            ICartsService _cartsService,
            IAddressesService _addressesService,
            IProductsService _productsService,
            IUsersService _usersService,
            INotificationsService _notificationsService,
            IHubContext<NotificationsHub> _notificationsHub)
        {
            ordersService = _ordersService;
            cartsService = _cartsService;
            addressesService = _addressesService;
            productsService = _productsService;
            usersService = _usersService;
            notificationsService = _notificationsService;
            notificationsHub = _notificationsHub;
        }
        public async Task<IActionResult> Index()
        {
            string userId = User.GetId();
            var deliveredOrders = await ordersService.GetByStatusAsync(OrderStatus.Delivered, userId);
            return View(deliveredOrders);
        }

        [HttpPost]
        public async Task<IActionResult> Make(MakeOrderInputModel model)
        {
            string cartViewLocation = "~/Views/Cart/Index.cshtml";
            string userId = User.GetId();
            if (!ModelState.IsValid)
            {
                var viewModel = await cartsService.GetAsync(userId);
                if (viewModel == null)
                {
                    return NotFound();
                }
                model.Addresses = await addressesService.GetAllForUserAsync(userId);
                viewModel.Input = model;
                return View(cartViewLocation, viewModel);
            }

            var addResult = await ordersService.AddAsync(model, userId);

            if (addResult.Result == MakeOrderResult.NotSpecifiedAddress)
            {
                var viewModel = await cartsService.GetAsync(userId);
                if (viewModel == null)
                {
                    return NotFound();
                }
                model.Addresses = await addressesService.GetAllForUserAsync(userId);
                viewModel.Input = model;
                ModelState.AddModelError(string.Empty, addResult.ErrorMessage ?? string.Empty);
                return View(cartViewLocation, viewModel);
            }

            if (addResult.Result == MakeOrderResult.InvalidAddress)
            {
                var viewModel = await cartsService.GetAsync(userId);
                if (viewModel == null)
                {
                    return NotFound();
                }
                model.Addresses = await addressesService.GetAllForUserAsync(userId);
                viewModel.Input = model;
                ModelState.AddModelError(string.Empty, addResult.ErrorMessage ?? string.Empty);
                return View(cartViewLocation, viewModel);
            }

            if (addResult.Result == MakeOrderResult.SomethingWentWrong)
            {
                var viewModel = await cartsService.GetAsync(userId);
                if (viewModel == null)
                {
                    return NotFound();
                }
                model.Addresses = await addressesService.GetAllForUserAsync(userId);
                viewModel.Input = model;
                ModelState.AddModelError(string.Empty, addResult.ErrorMessage ?? string.Empty);
                return View(cartViewLocation, viewModel);
            }

            if (addResult.Result == MakeOrderResult.InvalidQuantity)
            {
                var viewModel = await cartsService.GetAsync(userId);
                if (viewModel == null)
                {
                    return NotFound();
                }
                model.Addresses = await addressesService.GetAllForUserAsync(userId);
                viewModel.Input = model;
                ModelState.AddModelError(string.Empty, addResult.ErrorMessage ?? string.Empty);
                return View(cartViewLocation, viewModel);
            }

            if (addResult.Result == MakeOrderResult.NotFound)
            {
                return NotFound();
            }

            int cartId = await cartsService.GetIdAsync(userId);

            var removeResult = await cartsService.RemoveAllProductsAsync(cartId, userId);

            if (removeResult == AddUpdateDeleteResult.NotFound)
            {
                return NotFound();
            }

            if (removeResult == AddUpdateDeleteResult.Forbidden)
            {
                return Forbid();
            }


            var productsIds = model.ProductIds;
            var quantities = model.Quantities;
            var buyerEmail = await usersService.GetEmailAsync(userId);

            for (int i = 0; i < productsIds.Count; i++)
            {
                var ownerId = await productsService.GetOwnerIdAsync(model.ProductIds[i]);
                if (!string.IsNullOrWhiteSpace(ownerId))
                {
                    string notificationText = $"{buyerEmail} has just bought {quantities[i]} pieces of one of your products";
                    string productLink = $"/Products/Index/{productsIds[i]}";
                    var notificationId = await notificationsService.AddAsync(userId, ownerId, notificationText, productLink);
                    if (notificationId > 0)
                    {
                        await notificationsHub.Clients.User(ownerId).SendAsync("ReceiveNotification", notificationText, productLink, notificationId);
                    }
                }
            }

            TempData["SuccessfulOrder"] = "You have made a successful order. More details were sent to your mail.";

            return Redirect("/");
        }


        public async Task<IActionResult> Details(int id)
        {
            var userId = User.GetId();
            var adminUserId = await usersService.GetAdminIdAsync();
            var orderDetails = await ordersService.GetOrderDetailsAsync(id);
            if (orderDetails == null)
            {
                return NotFound();
            }

            if (orderDetails.UserId != userId && userId != adminUserId)
            {
                return Forbid();
            }

            return View(orderDetails);
        }
    }
}
