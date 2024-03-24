using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleShoppingApp.Models;
using System.Diagnostics;

namespace SimpleShoppingApp.Web.Controllers
{
    public class ErrorController : BaseController
    {
        [AllowAnonymous]
        [ActionName("404")]
        public IActionResult NotFoundError() 
        {
            return View();
        }

        [AllowAnonymous]
        [ActionName("500")]
        public IActionResult ServerError()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [AllowAnonymous]
        [ActionName("400")]
        public IActionResult BadRequestError()
        {
            return View();
        }
    }
}
