using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.Entities;
using DAL.Models; // Reference DAL Models

namespace BLL.Interface
{
    public interface IOrderService
    {
        Task CreatePaidOrderAsync(int userId, List<OrderDetail> details, Promotion? promotion);

        Task<List<OrderHistoryDto>> GetOrdersByDateAsync(DateTime from, DateTime to);
        Task<StatisticSummaryDto> GetStatisticsAsync(DateTime from, DateTime to);
    }
}