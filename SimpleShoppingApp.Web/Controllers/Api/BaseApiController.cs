using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SimpleShoppingApp.Web.Controllers.Api
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class BaseApiController : ControllerBase
    {
    }
}
