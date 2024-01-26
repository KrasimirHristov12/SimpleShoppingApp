﻿using SimpleShoppingApp.Models.Images;

namespace SimpleShoppingApp.Models.Products
{
    public class ProductViewModel
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public double Rating { get; set; }
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }

        public IEnumerable<ImageViewModel> Images { get; set; }
    }
}
