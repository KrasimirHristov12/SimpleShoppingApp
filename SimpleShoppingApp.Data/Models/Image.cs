using System.ComponentModel.DataAnnotations;

namespace SimpleShoppingApp.Data.Models
{
    public class Image
    { 
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        [Required]
        [MaxLength(6)]
        public string Extension { get; set; } = null!;

        public bool IsDeleted { get; set; }

    }
}
