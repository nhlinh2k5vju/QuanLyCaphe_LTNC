using System;
using System.Collections.Generic;

namespace UI.Models
{
    public class PaymentSummary
    {
        public int OrderId { get; set; }
        public string StaffName { get; set; } = "";
        public DateTime CreatedTime { get; set; }

        public List<PaymentItem> Items { get; set; } = new();

        public decimal SubTotal { get; set; }
        public decimal Discount { get; set; }
        public decimal Total { get; set; }
    }

    public class PaymentItem
    {
        public string MenuItemName { get; set; } = "";
        public string Size { get; set; } = "";
        public int Quantity { get; set; }
        public decimal Total { get; set; }
    }
}
