﻿using System.ComponentModel.DataAnnotations;

namespace Foody.Web.Models.Dto
{
    public class OrderHeaderDto
    {
       
        public int OrderHeaderId { get; set; }
        public string? UserId { get; set; }
        public string? CouponCode { get; set; }
        public double Discount { get; set; }
        public double OrderTotal { get; set; }
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Status { get; set; } // e.g., "Pending", "Completed", "Cancelled"
        public DateTime OrderTime { get; set; }

        public string? PaymentIntentId { get; set; }
        public string? StripeSessionId { get; set; }

        public IEnumerable<OrderDetailsDto> OrderDetails { get; set; }


    }
}
