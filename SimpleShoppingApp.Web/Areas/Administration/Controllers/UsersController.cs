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
            int usersPerPage = 100;
            var users = await usersService.GetUsersPerPageAsync(page, usersPerPage);
            var totalUsers = await usersService.GetUsersCountAsync();
            var model = new AdministrationUsersListViewModel
            {
                Users = users,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling((decimal)totalUsers / usersPerPage),
            };
            return View(model);
        }
    }
}
