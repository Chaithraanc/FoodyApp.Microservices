using Foody.Services.OrderAPI.Models.Dto;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Foody.Services.OrderAPI.Models
{
    public class OrderDetails
    {
        [Key]
        public int OrderDetailsId { get; set; }

        public int OrderHeaderId { get; set; }

        [ForeignKey("OrderHeaderId")]
        public OrderHeader? OrderHeader { get; set; }

        public int ProductId { get; set; }

        [NotMapped]
        public ProductDto? Product { get; set; }

        public int Count { get; set; }
        public double Price { get; set; } // Price of the product at the time of order

        public string ProductName { get; set; } = string.Empty; // Name of the product

    }
}
