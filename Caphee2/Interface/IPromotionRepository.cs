using System;
using System.Collections.Generic;
using System.Text;

using DAL.Entities;

namespace DAL.Interface
{
    public interface IPromotionRepository
    {
        Task<Promotion?> GetByCodeAsync(string code);
        Task<List<Promotion>> GetActivePromotionsAsync();
        Task AddAsync(Promotion promotion);
        Task UpdateAsync(Promotion promotion);
    }
}

