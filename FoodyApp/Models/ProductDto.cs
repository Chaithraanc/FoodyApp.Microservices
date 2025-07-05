using Foody.Web.Utility;
using System.ComponentModel.DataAnnotations;

namespace Foody.Web.Models
{
    public class ProductDto
    {
        
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public string? ImageUrl { get; set; }
        public string? ImageLocalPath { get; set; }

        [AllowedMaxFileSize(1)]
        [AllowedExtensions(new string[] { ".jpg", ".jpeg", ".png" })]
        public IFormFile? Image { get; set; }
        public string CategoryName { get; set; }

        [Range(1, 100, ErrorMessage = "Count must be between 1 and 100.")]
        public int Count { get; set; } = 1;

    }
}
