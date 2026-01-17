
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Entities
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        public DateTime OrderTime { get; set; } = DateTime.Now;

        [Required]
        public int UserId { get; set; }

        public int? PromotionId { get; set; }

        [Required]
        public decimal SubTotal { get; set; }

        public decimal DiscountAmount { get; set; }

        [Required]
        public decimal TotalAmount { get; set; }

        [Required]
        public string OrderStatus { get; set; } = "PAID";

        // Navigation
        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }

        [ForeignKey(nameof(PromotionId))]
        public Promotion? Promotion { get; set; }

        public ICollection<OrderDetail>? OrderDetails { get; set; }
    }
}

