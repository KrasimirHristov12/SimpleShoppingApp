using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SimpleShoppingApp.Web.Controllers
{
    [Authorize]
    public class BaseController : Controller
    {
    }
}
