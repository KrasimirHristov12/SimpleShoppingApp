using Microsoft.AspNetCore.Mvc;
using SimpleShoppingApp.Services.Categories;

namespace SimpleShoppingApp.Web.ViewComponents
{
    public class NavCategoriesViewComponent : ViewComponent
    {
        private readonly ICategoriesService categoriesService;

        public NavCategoriesViewComponent(ICategoriesService _categoriesService)
        {
            categoriesService = _categoriesService;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = await categoriesService.GetAllAsync();

            return View(model);
        }
    }
}
