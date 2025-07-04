using Repository.Models;
using Repository.Models.DTOs.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.IServices
{
    public interface IPromotionService
    {
        Task<Promotion> AddPromotion(PromotionRequest Request);
        Task DeletePromotion(Guid Id);
        Task<Promotion> UpdatePromotion(Guid Id, PromotionRequest Request);
        Task<IEnumerable<Promotion>> GetPromotion();
        Task<Promotion> GetPromotionById(Guid Id);
        Task<Promotion> GetPromotionByCode(string code);
    }
}
