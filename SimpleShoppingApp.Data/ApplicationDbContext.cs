using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SimpleShoppingApp.Data.Models;

namespace SimpleShoppingApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            var categoriesNames = new List<string>
            {
                "Electronics", "Clothing and Accessories", "Sports and Outdoors"
            };

            var categories = categoriesNames.Select(c => new Category
            {
                Id = categoriesNames.IndexOf(c) + 1,
                Name = c,
            }).ToList();


            builder.Entity<Category>().HasData(categories);

            builder.Entity<Product>()
                .Property(p => p.CategoryId)
                .HasDefaultValue(1);

            builder.Entity<ShoppingCart>()
                .HasOne(c => c.User)
                .WithOne(u => u.Cart)
                .HasForeignKey<ShoppingCart>(c => c.UserId);

            builder.Entity<Product>()
                .HasOne(p => p.User)
                .WithMany(u => u.Products)
                .HasForeignKey("UserId")
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ShippingAddress>()
                .HasMany(a => a.Orders)
                .WithOne(o => o.Address)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Order>()
                .Property(o => o.AddressId)
                .HasDefaultValue(1);

            base.OnModelCreating(builder);
        }
        public DbSet<Product> Products { get; set; } = null!;

        public DbSet<Image> Images { get; set; } = null!;

        public DbSet<Category> Categories { get; set; } = null!;

        public DbSet<CartsProducts> CartsProducts { get; set; } = null!;

        public DbSet<Order> Orders { get; set; } = null!;

        public DbSet<OrdersProducts> OrdersProducts { get; set; } = null!;

        public DbSet<ShippingAddress> Addresses { get; set; } = null!;
    }
}