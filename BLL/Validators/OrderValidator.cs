using System;
using System.Collections.Generic;
using System.Text;

using BLL.Exceptions;
using DAL.Entities;

namespace BLL.Validators
{
    public static class OrderValidator
    {
        public static void ValidateOrderDetails(List<OrderDetail> details)
        {
            if (details == null || !details.Any())
                throw new BusinessException("Hoá đơn phải có ít nhất một món.");

            foreach (var item in details)
            {
                if (item.Quantity <= 0)
                    throw new BusinessException("Số lượng món phải lớn hơn 0.");

                if (item.UnitPrice <= 0)
                    throw new BusinessException("Đơn giá món không hợp lệ.");

                if (string.IsNullOrWhiteSpace(item.Size))
                    throw new BusinessException("Size món không hợp lệ.");
            }
        }

        public static decimal CalculateSubTotal(List<OrderDetail> details)
        {
            return details.Sum(d => d.UnitPrice * d.Quantity);
        }

        public static decimal CalculateDiscount(Promotion? promo, decimal subTotal)
        {
            if (promo == null) return 0;

            return promo.DiscountType == "PERCENT"
                ? subTotal * promo.DiscountValue / 100
                : promo.DiscountValue;
        }
    }
}

