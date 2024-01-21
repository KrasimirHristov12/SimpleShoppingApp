using Microsoft.AspNetCore.Mvc;

namespace SimpleShoppingApp.Controllers
{
    public class CartController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
