using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BLL.Interface;
using DAL.Entities;
using DAL.Interface;
using DAL.Models; // Sử dụng DTO từ DAL

namespace BLL.Implementation
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepo;

        public OrderService(IOrderRepository orderRepo)
        {
            _orderRepo = orderRepo;
        }

        //  THANH TOÁN 
        public async Task CreatePaidOrderAsync(
            int userId,
            List<OrderDetail> orderDetails,
            Promotion? promotion)
        {
            decimal subTotal = 0;
            if (orderDetails != null)
            {
                subTotal = orderDetails.Sum(x => x.Quantity * x.UnitPrice);
            }

            decimal discountAmount = 0;

            if (promotion != null && promotion.DiscountValue > 0)
            {
                if (promotion.DiscountType == "PERCENT")
                {
                    discountAmount = subTotal * (promotion.DiscountValue / 100m);
                }
                else
                {
                    discountAmount = promotion.DiscountValue;
                }

                if (discountAmount > subTotal)
                {
                    discountAmount = subTotal;
                }
            }

            // 3. Tạo đối tượng Order
            var order = new Order
            {
                UserId = userId,
                PromotionId = promotion?.PromotionId,
                OrderTime = DateTime.Now,

                SubTotal = subTotal,
                DiscountAmount = discountAmount,
                TotalAmount = subTotal - discountAmount,

                OrderStatus = "PAID",
                OrderDetails = orderDetails
            };

            await _orderRepo.AddOrderAsync(order);
        }

        // ================= LỊCH SỬ GIAO DỊCH =================
        public async Task<List<OrderHistoryDto>> GetOrdersByDateAsync(DateTime from, DateTime to)
        {
            if (from > to)
            {
                throw new ArgumentException("Thời gian bắt đầu không thể lớn hơn thời gian kết thúc.");
            }

            return await _orderRepo.GetOrderHistoryAsync(from, to);
        }


        public async Task<StatisticSummaryDto> GetStatisticsAsync(DateTime from, DateTime to)
        {
            return await _orderRepo.GetStatisticsAsync(from, to);
        }
    }
}