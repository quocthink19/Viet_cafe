using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public interface IPromotionUsedRepo : IRepository<PromotionUsed>
    {
        Task<bool> CheckExsit(Guid promotionId, Guid customerId);
        Task<int> CountPromotionUsedAsync(Guid promotionId);
    }
}
