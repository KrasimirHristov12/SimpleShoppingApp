using System.ComponentModel.DataAnnotations;

namespace SimpleShoppingApp.Data.Models
{
    public class Image
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        [MaxLength(6)]
        public string? Extension { get; set; }

        public string? ImageUrl { get; set; }

        public bool IsDeleted { get; set; }

        public int ProductId { get; set; }

        public Product Product { get; set; } = null!;

    }
}
