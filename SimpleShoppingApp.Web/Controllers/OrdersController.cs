using Microsoft.AspNetCore.Mvc;
using SimpleShoppingApp.Data.Enums;
using SimpleShoppingApp.Models.Addresses;
using SimpleShoppingApp.Models.Orders;
using SimpleShoppingApp.Services.Addresses;
using SimpleShoppingApp.Services.Carts;
using SimpleShoppingApp.Services.Orders;
using SimpleShoppingApp.Extensions;
using SimpleShoppingApp.Services.Notifications;
using SimpleShoppingApp.Services.Products;

namespace SimpleShoppingApp.Web.Controllers
{
    public class OrdersController : BaseController
    {
        private readonly IOrdersService ordersService;
        private readonly ICartsService cartsService;
        private readonly IAddressesService addressesService;

        public OrdersController(IOrdersService _ordersService,
            ICartsService _cartsService,
            IAddressesService _addressesService,
            INotificationsService _notificationsService,
            IProductsService _productsService)
        {
            ordersService = _ordersService;
            cartsService = _cartsService;
            addressesService = _addressesService;
        }
        public async Task<IActionResult> Index()
        {
            string userId = User.GetId();
            var deliveredOrders = await ordersService.GetByStatusAsync(OrderStatus.Delivered, userId);
            return View(deliveredOrders);
        }

        public async Task<IActionResult> GetByStatus(int status)
        {
            if (!Enum.IsDefined(typeof(OrderStatus), status))
            {
                return NotFound();
                
            }
            string userId = User.GetId();

            var orderStatus = (OrderStatus)status;

            var orders = await ordersService.GetByStatusAsync(orderStatus, userId);

            return Json(orders);

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


            return Redirect("/");
        }


        public async Task<IActionResult> Details(int id)
        {
            var orderDetails = await ordersService.GetOrderDetailsAsync(id);
            if (orderDetails == null)
            {
                return NotFound();
            }
            return View(orderDetails);
        }
    }
}
