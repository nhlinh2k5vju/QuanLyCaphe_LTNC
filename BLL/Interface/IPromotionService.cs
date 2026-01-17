using System;
using System.Collections.Generic;
using System.Text;

using DAL.Entities;

namespace BLL.Interface
{
    public interface IPromotionService
    {
        Task<List<Promotion>> GetActivePromotionsAsync();
        Task<Promotion?> ValidatePromotionAsync(string code, decimal subTotal);
        Task AddAsync(Promotion promotion);
        Task UpdateAsync(Promotion promotion);
    }

}

