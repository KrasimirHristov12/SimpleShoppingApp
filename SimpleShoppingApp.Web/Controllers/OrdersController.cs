using Microsoft.AspNetCore.Mvc;
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
        public IActionResult Index()
        {
            return View();
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
