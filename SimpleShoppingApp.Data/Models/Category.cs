using System.ComponentModel.DataAnnotations;

namespace SimpleShoppingApp.Data.Models
{
    public class Category
    {
        public Category()
        {
            this.Products = new HashSet<Product>();
        }
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string? Name { get; set; }

        public ICollection<Product> Products { get; set; }
    }
}
