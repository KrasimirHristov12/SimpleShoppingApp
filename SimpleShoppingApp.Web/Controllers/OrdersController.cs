using Microsoft.AspNetCore.Mvc;
using SimpleShoppingApp.Data.Enums;
using SimpleShoppingApp.Models.Addresses;
using SimpleShoppingApp.Models.Orders;
using SimpleShoppingApp.Services.Addresses;
using SimpleShoppingApp.Services.Carts;
using SimpleShoppingApp.Services.Orders;
using SimpleShoppingApp.Services.Users;

namespace SimpleShoppingApp.Web.Controllers
{
    public class OrdersController : BaseController
    {
        private readonly IOrdersService ordersService;
        private readonly IUsersService usersService;
        private readonly ICartsService cartsService;
        private readonly IAddressesService addressesService;

        public OrdersController(IOrdersService _ordersService, 
            IUsersService _usersService,
            ICartsService _cartsService,
            IAddressesService _addressesService)
        {
            ordersService = _ordersService;
            usersService = _usersService;
            cartsService = _cartsService;
            addressesService = _addressesService;
        }
        public async Task<IActionResult> Index()
        {
            string userId = usersService.GetId(User);
            var deliveredOrders = await ordersService.GetByStatusAsync(OrderStatus.Delivered, userId);
            return View(deliveredOrders);
        }

        public async Task<IActionResult> GetByStatus(int status)
        {
            if (!Enum.IsDefined(typeof(OrderStatus), status))
            {
                return BadRequest();
                
            }
            string userId = usersService.GetId(User);

            var orderStatus = (OrderStatus)status;

            var orders = await ordersService.GetByStatusAsync(orderStatus, userId);

            return Json(orders);

        }

        [HttpPost]
        public async Task<IActionResult> Make(MakeOrderInputModel model)
        {
            //if (!ModelState.IsValid)
            //{
            //    return View("~/Views/Cart/Index.cshtml", model);
            //}
            string userId = usersService.GetId(User);

            var result = await ordersService.AddAsync(model, userId);

            if (!result)
            {
                return BadRequest();
            }

            int? cartId = await cartsService.GetIdAsync(userId);

            if (cartId == null)
            {
                return NotFound();
            }

            await cartsService.RemoveAllProductsAsync((int)cartId);

            return Redirect("/");
        }

        [HttpPost]
        public async Task<IActionResult> AddAddress(AddAddressInputModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            string userId = usersService.GetId(User);
            var addedAddress = await addressesService.AddAsync(model.Name, userId);
            return Json(addedAddress);
        }

        public async Task<IActionResult> Details(int id)
        {
            return View();
        }
    }
}
