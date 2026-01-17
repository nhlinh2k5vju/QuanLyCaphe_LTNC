
using System;

namespace DAL.Models 
{
    public class OrderHistoryDto
    {
        public int OrderId { get; set; }
        public DateTime OrderTime { get; set; }
        public string StaffName { get; set; } = string.Empty;
        public string MenuItemName { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal => Quantity * UnitPrice; 
        public decimal OrderTotal { get; set; }
    }
}