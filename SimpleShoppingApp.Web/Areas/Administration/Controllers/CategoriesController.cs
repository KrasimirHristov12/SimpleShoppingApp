using Microsoft.AspNetCore.Mvc;
using SimpleShoppingApp.Models.Categories;
using SimpleShoppingApp.Services.Categories;
using static SimpleShoppingApp.Data.Constants.GlobalConstants;

namespace SimpleShoppingApp.Web.Areas.Administration.Controllers
{
    public class CategoriesController : AdministrationBaseController
    {
        private readonly ICategoriesService categoriesService;

        public CategoriesController(ICategoriesService _categoriesService)
        {
            categoriesService = _categoriesService;
        }
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddCategoryInputModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await categoriesService.AddAsync(model.Name);

            if (!result)
            {
                ModelState.AddModelError(string.Empty, AddCategoryErrorMessage);
                return View(model);
            }

            TempData["CategoryCreated"] = "You have successfully created a new category";

            return RedirectToAction("Index", "Home", new { area = "" });

        }
    }
}
