using Foody.Services.ShoppingCartAPI.Models.Dto;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Foody.Services.ShoppingCartAPI.Models
{
    public class CartDetails
    {
        [Key]
        public int CartDetailsId { get; set; }

        public int CartHeaderId { get; set; }

        [ForeignKey("CartHeaderId")]
        public CartHeader CartHeader { get; set; }

        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        [NotMapped]
        public ProductDto Product { get; set; }

        public int Count { get; set; }
    }
}
