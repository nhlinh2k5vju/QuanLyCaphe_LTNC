using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DAL.Entities;
using DAL.Interface;

namespace DAL.Implementation
{
    public class PromotionRepository : IPromotionRepository
    {
        private readonly CoffeDbContext _context;

        public PromotionRepository(CoffeDbContext context)
        {
            _context = context;
        }

        public async Task<Promotion?> GetByCodeAsync(string code)
        {
            return await _context.Promotions
                .FirstOrDefaultAsync(p =>
                    p.Code == code &&
                    p.Status);
        }

        public async Task<List<Promotion>> GetActivePromotionsAsync()
        {
            return await _context.Promotions
                .Where(p =>
                    p.Status &&
                    p.StartDate <= DateTime.Now &&
                    p.EndDate >= DateTime.Now)
                .ToListAsync();
        }

        public async Task AddAsync(Promotion promotion)
        {
            await _context.Promotions.AddAsync(promotion);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Promotion promotion)
        {
            _context.Promotions.Update(promotion);
            await _context.SaveChangesAsync();
        }
    }
}
