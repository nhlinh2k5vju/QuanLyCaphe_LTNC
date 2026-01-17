using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Entities;
using DAL.Interface;
using DAL.Models; // Sử dụng DTO của DAL
using Microsoft.EntityFrameworkCore;

namespace DAL.Implementation
{
    public class OrderRepository : IOrderRepository
    {
        private readonly CoffeDbContext _context;

        public OrderRepository(CoffeDbContext context)
        {
            _context = context;
        }

        public async Task AddOrderAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
        }

        public async Task<Order?> GetOrderDetailAsync(int orderId)
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderDetails)
                    .ThenInclude(d => d.MenuItem)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }

        public async Task<List<OrderHistoryDto>> GetOrderHistoryAsync(DateTime from, DateTime to)
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderDetails)
                    .ThenInclude(d => d.MenuItem)
                // Filter ngày tháng
                .Where(o => o.OrderTime.Date >= from.Date && o.OrderTime.Date <= to.Date)
                .SelectMany(o => o.OrderDetails.Select(d => new OrderHistoryDto
                {
                    OrderId = o.OrderId,
                    OrderTime = o.OrderTime,
                    StaffName = o.User != null ? o.User.FullName : "N/A",

                    MenuItemName = d.MenuItem.Name,
                    Size = d.Size,
                    Quantity = d.Quantity,
                    UnitPrice = d.UnitPrice,

                    // LineTotal tự tính trong class DTO hoặc tính ở đây:
                    // LineTotal = d.Quantity * d.UnitPrice, 
                    OrderTotal = o.TotalAmount
                }))
                .OrderByDescending(x => x.OrderTime) // Nên sắp xếp mới nhất lên đầu
                .ToListAsync();
        }

        public async Task<StatisticSummaryDto> GetStatisticsAsync(
    DateTime from,
    DateTime to)
        {
            var orders = await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(d => d.MenuItem)
                .Where(o => o.OrderTime.Date >= from.Date &&
                            o.OrderTime.Date <= to.Date)
                .ToListAsync();

            var result = new StatisticSummaryDto
            {
                OrderCount = orders.Count,
                TotalRevenue = orders.Sum(o => o.TotalAmount)
            };

            // TOP 3 MÓN BÁN CHẠY
            result.TopProducts = orders
                .SelectMany(o => o.OrderDetails)
                .GroupBy(d => new { d.MenuItem.Name, d.Size })
                .Select(g => new TopProductDto
                {
                    MenuItemName = g.Key.Name,
                    Size = g.Key.Size,
                    Quantity = g.Sum(x => x.Quantity)
                })
                .OrderByDescending(x => x.Quantity)
                .Take(3)
                .ToList();

            return result;
        }

    }
}