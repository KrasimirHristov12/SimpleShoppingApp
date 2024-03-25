using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SimpleShoppingApp.Data;
using SimpleShoppingApp.Data.Models;
using SimpleShoppingApp.Data.Repository;
using SimpleShoppingApp.Data.Seeders;
using SimpleShoppingApp.Services.Addresses;
using SimpleShoppingApp.Services.Carts;
using SimpleShoppingApp.Services.Categories;
using SimpleShoppingApp.Services.Emails;
using SimpleShoppingApp.Services.Images;
using SimpleShoppingApp.Services.Notifications;
using SimpleShoppingApp.Services.Orders;
using SimpleShoppingApp.Services.Products;
using SimpleShoppingApp.Services.Users;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;

}).AddRoles<IdentityRole>()
   .AddEntityFrameworkStores<ApplicationDbContext>()
   .AddDefaultTokenProviders();
builder.Services.AddControllersWithViews();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
});

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IProductsService, ProductsService>();
builder.Services.AddScoped<IImagesService, ImagesService>();
builder.Services.AddScoped<ICategoriesService, CategoriesService>();
builder.Services.AddScoped<UserSeeder>();
builder.Services.AddScoped<AdminRoleSeeder>();
builder.Services.AddScoped<ProductsSeeder>();
builder.Services.AddScoped<CategoriesSeeder>();
builder.Services.AddScoped<IUsersService, UsersService>();
builder.Services.AddScoped<ICartsService, CartsService>();
builder.Services.AddScoped<IOrdersService, OrdersService>();
builder.Services.AddScoped<IAddressesService, AddressesService>();
builder.Services.AddScoped<IEmailsService, EmailsService>();
builder.Services.AddScoped<INotificationsService, NotificationsService>();


var app = builder.Build();

using (var serviceScope = app.Services.CreateScope())
{
    var adminRoleSeeder = serviceScope.ServiceProvider.GetRequiredService<AdminRoleSeeder>();
    var userSeeder = serviceScope.ServiceProvider.GetRequiredService<UserSeeder>();
    var productsSeeder = serviceScope.ServiceProvider.GetRequiredService<ProductsSeeder>();
    var categoriesSeeder = serviceScope.ServiceProvider.GetRequiredService<CategoriesSeeder>();
    //adminRoleSeeder.SeedAsync().GetAwaiter().GetResult();
    //userSeeder.SeedAsync().GetAwaiter().GetResult();
    await categoriesSeeder.SeedAsync();
    await productsSeeder.SeedAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error/500");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/Error/{0}");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
      name: "areas",
      pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
    );
});


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();
app.Run();
