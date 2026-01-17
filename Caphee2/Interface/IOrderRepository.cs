using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.Entities;
using DAL.Models; 

namespace DAL.Interface
{
    public interface IOrderRepository
    {
        Task AddOrderAsync(Order order);
        Task<List<OrderHistoryDto>> GetOrderHistoryAsync(DateTime from, DateTime to);
        Task<Order?> GetOrderDetailAsync(int orderId);
        Task<StatisticSummaryDto> GetStatisticsAsync(DateTime from, DateTime to);
    }
}