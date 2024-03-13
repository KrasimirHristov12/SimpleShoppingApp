using System.Security.Claims;

namespace SimpleShoppingApp.Extensions
{
    public static class ClaimsPrincipalExtension
    {
        public static string GetId(this ClaimsPrincipal user)
        {
            return user.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
        }
    }
}
