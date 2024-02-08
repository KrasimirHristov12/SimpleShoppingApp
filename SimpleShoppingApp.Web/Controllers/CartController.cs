using Microsoft.AspNetCore.Mvc;

namespace SimpleShoppingApp.Web.Controllers
{
    public class CartController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
