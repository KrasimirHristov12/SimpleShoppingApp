using System.ComponentModel.DataAnnotations;

namespace SimpleShoppingApp.Data.Models
{
    public class Image
    {
        public Image()
        {
            Name = string.Empty;
            Extension = string.Empty;
        }
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [MaxLength(6)]
        public string Extension { get; set; }

        public bool IsDeleted { get; set; }

    }
}
