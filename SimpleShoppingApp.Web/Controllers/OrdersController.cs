using Microsoft.AspNetCore.Mvc;
using SimpleShoppingApp.Data.Enums;
using SimpleShoppingApp.Models.Orders;
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

        public OrdersController(IOrdersService _ordersService, IUsersService _usersService, ICartsService _cartsService)
        {
            ordersService = _ordersService;
            usersService = _usersService;
            cartsService = _cartsService;
        }
        public async Task<IActionResult> Index()
        {
            string? userId = usersService.GetId(User);
            if (userId == null)
            {
                return Unauthorized();
            }
            var deliveredOrders = await ordersService.GetByStatusAsync(OrderStatus.Delivered, userId);
            return View(deliveredOrders);
        }

        public async Task<IActionResult> GetByStatus(int status)
        {
            if (!Enum.IsDefined(typeof(OrderStatus), status))
            {
                return BadRequest();
                
            }

            string? userId = usersService.GetId(User);

            if (userId == null)
            {
                return Unauthorized();
            }

            var orderStatus = (OrderStatus)status;

            var orders = await ordersService.GetByStatusAsync(orderStatus, userId);

            return Json(orders);

        }

        [HttpPost]
        public async Task<IActionResult> Make(MakeOrderInputModel model)
        {
            string? userId = usersService.GetId(User);

            if (userId == null)
            {
                return Unauthorized();
            }

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
    }
}
