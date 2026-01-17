using System;

namespace UI.Models
{
    public class OrderHistoryDto
    {
        public int OrderId { get; set; }
        public DateTime CreatedTime { get; set; }
        public string StaffName { get; set; } = "";

        public string MenuItemName { get; set; } = "";
        public string Size { get; set; } = "";
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        public decimal LineTotal => Quantity * UnitPrice;

        public decimal OrderTotal { get; set; }
    }
}
