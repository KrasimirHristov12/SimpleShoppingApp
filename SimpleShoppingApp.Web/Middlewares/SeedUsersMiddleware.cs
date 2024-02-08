using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimpleShoppingApp.Data.Seeders;

namespace SimpleShoppingApp.Web.Middlewares
{
    public class SeedUsersMiddleware
    {
        private readonly RequestDelegate next;

        public SeedUsersMiddleware(RequestDelegate _next)
        {
            next = _next;
        }

        public async Task InvokeAsync(HttpContext context, UserSeeder seeder)
        {
            await seeder.SeedAsync();
            await next(context);
        }
    }

    public static class SeedUsersMiddlewareExtensions
    {
        public static IApplicationBuilder UseUsersSeeder(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SeedUsersMiddleware>();
        }
    }
}
