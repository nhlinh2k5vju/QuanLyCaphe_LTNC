using System;
using System.Collections.Generic;
using System.Text;

using BLL.Exceptions;
using DAL.Entities;

namespace BLL.Validators
{
    public static class PromotionValidator
    {
        public static void Validate(Promotion promo, decimal subTotal)
        {
            if (!promo.Status)
                throw new BusinessException("Mã giảm giá đã bị vô hiệu.");

            if (DateTime.Now < promo.StartDate || DateTime.Now > promo.EndDate)
                throw new BusinessException("Mã giảm giá đã hết hạn.");

            if (promo.DiscountValue <= 0)
                throw new BusinessException("Giá trị giảm giá không hợp lệ.");

            if (promo.DiscountType == "PERCENT" && promo.DiscountValue > 100)
                throw new BusinessException("Giảm giá phần trăm không hợp lệ.");

            if (subTotal <= 0)
                throw new BusinessException("Đơn hàng không hợp lệ để áp mã.");
        }
    }
}

