﻿using System.ComponentModel.DataAnnotations;

namespace SimpleShoppingApp.Data.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string? Name { get; set; }

        [Required]
        public string? Description { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public double Rating { get; set; }
    }
}
