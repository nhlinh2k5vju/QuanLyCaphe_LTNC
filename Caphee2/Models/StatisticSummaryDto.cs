using System;
using System.Collections.Generic;
using System.Text;
using DAL.Models;

namespace DAL.Models
{
    public class StatisticSummaryDto
    {
        public int OrderCount { get; set; }
        public decimal TotalRevenue { get; set; }
        public List<TopProductDto> TopProducts { get; set; } = new();
    }

}
