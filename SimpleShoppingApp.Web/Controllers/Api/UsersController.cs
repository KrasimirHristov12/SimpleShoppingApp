using Microsoft.AspNetCore.Mvc;
using SimpleShoppingApp.Models.Users;
using SimpleShoppingApp.Services.Users;
using SimpleShoppingApp.Extensions;
using static SimpleShoppingApp.Data.Constants.GlobalConstants;

namespace SimpleShoppingApp.Web.Controllers.Api
{
    public class UsersController : BaseApiController
    {
        private readonly IUsersService usersService;

        public UsersController(IUsersService _usersService)
        {
            usersService = _usersService;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult<EditFullNameInputModel>> EditFullName(EditFullNameInputModel model)
        {
            var userId = User.GetId();
            var editResult = await usersService.UpdateFullNameAsync(model, userId);
            if (!editResult)
            {
                return NotFound();
            }
            return model;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult<EditPhoneNumberInputModel>> EditPhoneNumber(EditPhoneNumberInputModel model)
        {
            var userId = User.GetId();
            var editResult = await usersService.UpdatePhoneNumberAsync(model, userId);
            if (!editResult)
            {
                return NotFound();
            }
            return model;
        }

    }
}
