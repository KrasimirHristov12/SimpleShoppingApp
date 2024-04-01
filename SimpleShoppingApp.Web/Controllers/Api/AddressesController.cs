using Microsoft.AspNetCore.Mvc;
using SimpleShoppingApp.Extensions;
using SimpleShoppingApp.Models.Addresses;
using SimpleShoppingApp.Services.Addresses;

namespace SimpleShoppingApp.Web.Controllers.Api
{
    public class AddressesController : BaseApiController
    {
        private readonly IAddressesService addressesService;

        public AddressesController(IAddressesService _addressesService)
        {
            addressesService = _addressesService;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult<AddressViewModel>> AddAddress([FromForm]AddAddressInputModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            string userId = User.GetId();
            var addedAddress = await addressesService.AddAsync(model.Name, userId);
            return Ok(addedAddress);
        }
    }
}
