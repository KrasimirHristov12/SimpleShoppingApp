using Microsoft.AspNetCore.Mvc;
using SimpleShoppingApp.Models.Addresses;

namespace SimpleShoppingApp.Web.ViewComponents
{
    public class AddressFormViewComponent : ViewComponent
    {
        public  IViewComponentResult Invoke()
        {
            var model = new AddAddressInputModel();
            return View(model);
        }
    }
}
