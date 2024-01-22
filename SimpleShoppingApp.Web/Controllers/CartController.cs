using Microsoft.AspNetCore.Mvc;

namespace SimpleShoppingApp.Web.Controllers
{
    public class CartController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
