using System;
using System.Collections.Generic;
using System.Text;

using System.ComponentModel.DataAnnotations;

namespace DAL.Entities
{
    public class Promotion
    {
        [Key]
        public int PromotionId { get; set; }

        [Required]
        [MaxLength(30)]
        public string Code { get; set; } = string.Empty;

        [Required]
        public string DiscountType { get; set; } = "PERCENT"; // PERCENT / AMOUNT

        [Required]
        public decimal DiscountValue { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public bool Status { get; set; } = true;

        // Navigation
        public ICollection<Order>? Orders { get; set; }
    }
}

