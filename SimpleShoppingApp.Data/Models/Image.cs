using System.ComponentModel.DataAnnotations;

namespace SimpleShoppingApp.Data.Models
{
    public class Image
    { 
        public int Id { get; set; }

        [Required]
        public string? Name { get; set; }

        [Required]
        [MaxLength(6)]
        public string? Extension { get; set; }

    }
}
