using Microsoft.AspNetCore.Mvc;

namespace SimpleShoppingApp.Controllers
{
    public class ProductsController : Controller
    {
        [Route("/[controller]/{id}")]
        public IActionResult Index(int id)
        {
            // Get product from the db

            // Transform it to vm & pass it to the view

            return View();
        }

        [Route("/[controller]/[action]/{name}")]
        public IActionResult Category(string name)
        {
            // Get category products - Get count, Implement Paging, filters (like in Emag)

            return View();
        }
    }
}
