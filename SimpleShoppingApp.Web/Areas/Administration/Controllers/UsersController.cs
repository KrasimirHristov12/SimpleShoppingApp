using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SimpleShoppingApp.Models.Users;
using SimpleShoppingApp.Services.Users;

namespace SimpleShoppingApp.Web.Areas.Administration.Controllers
{
    public class UsersController : AdministrationBaseController
    {
        private readonly IUsersService usersService;

        public UsersController(IUsersService _usersService)
        {
            usersService = _usersService;
        }
        public async Task<IActionResult> List(int page)
        {
            if (page < 1)
            {
                return BadRequest();
            }

            int usersPerPage = 100;
            var users = await usersService.GetUsersPerPageAsync(page, usersPerPage);

            if (users.Count() == 0)
            {
                return BadRequest();
            }

            var totalUsers = await usersService.GetUsersCountAsync();
            var model = new AdministrationUsersListViewModel
            {
                Users = users,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling((decimal)totalUsers / usersPerPage),
            };
            return View(model);
        }

        public async Task<IActionResult> Edit(string id)
        {
            var user = await usersService.GetEditUserAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(AdministrationUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            bool result = await usersService.EditUserAsync(model);

            if (!result)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(List), new { page = 1 });


        }
    }
}
