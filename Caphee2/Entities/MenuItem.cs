using System;
using System.Collections.Generic;
using System.Text;

using System.ComponentModel.DataAnnotations;

namespace DAL.Entities
{
    public class MenuItem
    {
        [Key]
        public int MenuItemId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public string? ImagePath { get; set; }

        public bool Status { get; set; } = true;

        // Navigation
        public ICollection<MenuItemSize>? MenuItemSizes { get; set; }
        public ICollection<OrderDetail>? OrderDetails { get; set; }
    }
}

