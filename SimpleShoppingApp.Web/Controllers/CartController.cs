using Microsoft.AspNetCore.Mvc;
using SimpleShoppingApp.Services.Carts;
using SimpleShoppingApp.Services.Products;
using SimpleShoppingApp.Services.Users;

namespace SimpleShoppingApp.Web.Controllers
{
    public class CartController : BaseController
    {
        private readonly ICartsService cartService;
        private readonly IUsersService usersService;
        private readonly IProductsService productsService;

        public CartController(ICartsService _cartService, IUsersService _usersService, IProductsService _productsService)
        {
            cartService = _cartService;
            usersService = _usersService;
            productsService = _productsService;
        }
        public async Task<IActionResult> Index()
        {
            var controllerName = "Cart";
            var actionName = "Index";
            var userId = usersService.GetId(User);
            if (userId == null)
            {
                return Redirect($"/Identity/Login?returnUrl=/{controllerName}/{actionName}");
            }
            if (!await cartService.DoesUserHaveCartAsync(userId))
            {
                await cartService.AddAsync(userId);
            }

            var cart = await cartService.GetAsync(userId);

            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int id)
        {
            if (!await productsService.DoesProductExistAsync(id))
            {
                return NotFound();
            }
            var controllerName = "Cart";
            var actionName = "Index";
            var userId = usersService.GetId(User);
            if (userId == null)
            {
                return Redirect($"/Identity/Login?returnUrl=/{controllerName}/{actionName}");
            }
            var cartId = await cartService.GetIdAsync(userId);
            if (cartId == 0)
            {
                return NotFound();
            }
            await cartService.AddProductAsync(cartId, id);
            return RedirectToAction(nameof(Index));
        }
    }
}
