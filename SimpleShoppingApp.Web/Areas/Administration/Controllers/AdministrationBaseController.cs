using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SimpleShoppingApp.Web.Areas.Administration.Controllers
{
    [Authorize(Roles = "Administrator")]
    [Area("Administration")]
    public class AdministrationBaseController : Controller
    {
    }
}
