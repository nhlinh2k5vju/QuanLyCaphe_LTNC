using System;
using System.Collections.Generic;
using System.Text;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Entities
{
    public class MenuItemSize
    {
        [Key]
        public int MenuItemSizeId { get; set; }

        [Required]
        public int MenuItemId { get; set; }

        [Required]
        [MaxLength(1)]
        public string Size { get; set; } = "M"; // S / M / L

        [Required]
        public decimal Price { get; set; }

        // Navigation
        [ForeignKey(nameof(MenuItemId))]
        public MenuItem? MenuItem { get; set; }
    }
}

