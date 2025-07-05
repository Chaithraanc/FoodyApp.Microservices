using System.ComponentModel.DataAnnotations;

namespace Foody.Services.ProductAPI.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required]
        public string Name { get; set; }
        public string Description { get; set; }

        [Range(0 , 1000)]
        public double Price { get; set; }
        public string ImageUrl { get; set; }

        public string? ImageLocalPath { get; set; }

     
        public string CategoryName { get; set; }


    
    }
}
