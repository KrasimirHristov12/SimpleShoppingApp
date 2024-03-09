using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SimpleShoppingApp.Web.Controllers;

namespace SimpleShoppingApp.Web.Filters
{
    public class NotAllowLoggedUsersAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.HttpContext.User.Identity?.IsAuthenticated ?? false)
            {
                context.Result = new RedirectToActionResult(nameof(HomeController.Index), "Home", null);
            }
            base.OnActionExecuting(context);
        }
    }
}
