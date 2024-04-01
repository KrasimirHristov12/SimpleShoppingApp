using Microsoft.AspNetCore.Mvc;
using SimpleShoppingApp.Data.Enums;
using SimpleShoppingApp.Extensions;
using SimpleShoppingApp.Services.Orders;

namespace SimpleShoppingApp.Web.Controllers.Api
{
    public class OrdersController : BaseApiController
    {
        private readonly IOrdersService ordersService;

        public OrdersController(IOrdersService _ordersService)
        {
            ordersService = _ordersService;
        }

        [Route("[action]")]
        public async Task<IActionResult> GetByStatus(int status)
        {
            if (!Enum.IsDefined(typeof(OrderStatus), status))
            {
                return NotFound();
            }
            string userId = User.GetId();

            var orderStatus = (OrderStatus)status;

            var orders = await ordersService.GetByStatusAsync(orderStatus, userId);

            return Ok(orders);

        }

    }
}
