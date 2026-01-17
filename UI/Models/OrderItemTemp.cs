using System;
using System.Collections.Generic;
using System.Text;

namespace UI.Models
{
    public class OrderItemTemp
    {
        public int MenuItemId { get; set; }
        public string MenuItemName { get; set; }
        public string Size { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }

        public decimal Total => UnitPrice * Quantity;
    }
}

