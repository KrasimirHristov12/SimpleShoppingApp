using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SimpleShoppingApp.Data.Models;

namespace SimpleShoppingApp.Data
{
    public class ApplicationDbContext : IdentityDbContext
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

            base.OnModelCreating(builder);
        }
        public DbSet<Product> Products { get; set; }

        public DbSet<Image> Images { get; set; }

        public DbSet<Category> Categories { get; set; }
    }
}