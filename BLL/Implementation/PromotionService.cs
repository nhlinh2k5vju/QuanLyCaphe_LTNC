using System.Collections.Generic;
using System.Threading.Tasks;
using BLL.Interface;
using BLL.Validators;
using DAL.Entities;
using DAL.Interface;

namespace BLL.Implementation
{
    public class PromotionService : IPromotionService
    {
        private readonly IPromotionRepository _promoRepo;

        public PromotionService(IPromotionRepository promoRepo)
        {
            _promoRepo = promoRepo;
        }

        // ================= QUERY =================
        public async Task<List<Promotion>> GetActivePromotionsAsync()
        {
            return await _promoRepo.GetActivePromotionsAsync();
        }

        // ================= VALIDATE =================
        public async Task<Promotion?> ValidatePromotionAsync(
            string code,
            decimal subTotal)
        {
            if (string.IsNullOrWhiteSpace(code))
                return null;

            var promo = await _promoRepo.GetByCodeAsync(code);

            if (promo == null)
                return null;

            PromotionValidator.Validate(promo, subTotal);
            return promo;
        }

        // ================= CRUD =================
        public async Task AddAsync(Promotion promotion)
        {
            await _promoRepo.AddAsync(promotion);
        }

        public async Task UpdateAsync(Promotion promotion)
        {
            await _promoRepo.UpdateAsync(promotion);
        }
    }
}
