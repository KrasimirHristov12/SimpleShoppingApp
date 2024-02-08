using SimpleShoppingApp.Data.Seeders;

namespace SimpleShoppingApp.Web.Middlewares
{
    public class SeedAdminRoleMiddleware
    {
        private readonly RequestDelegate next;

        public SeedAdminRoleMiddleware(RequestDelegate _next)
        {
            next = _next;
        }

        public async Task InvokeAsync(HttpContext context, AdminRoleSeeder seeder)
        {
            await seeder.SeedAsync();
            await next(context);
        }
    }

    public static class SeedAdminRoleMiddlewareExtensions
    {
        public static IApplicationBuilder UseAdminRoleSeeder(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SeedAdminRoleMiddleware>();
        }
    }
}
