﻿using System.ComponentModel.DataAnnotations;

namespace Foody.Services.ShoppingCartAPI.Models.Dto
{
    public class ProductDto
    {
        
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public string? ImageUrl { get; set; }

        public string? ImageLocalPath { get; set; }

        public IFormFile? Image { get; set; }
        public string CategoryName { get; set; }
    }
}
