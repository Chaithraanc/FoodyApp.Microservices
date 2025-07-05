using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Foody.Web.Models.Dto
{
    public class OrderDetailsDto
    {
   
        public int OrderDetailsId { get; set; }

        public int OrderHeaderId { get; set; }

        public int ProductId { get; set; }

        public int Count { get; set; }
        public double Price { get; set; } // Price of the product at the time of order

        public string ProductName { get; set; } = string.Empty; // Name of the product
    }
}
