using Microsoft.EntityFrameworkCore;
using Repository.IRepository;
using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories
{
    public class PromotionUsedRepo : Repository<PromotionUsed>, IPromotionUsedRepo
    {
        private readonly ApplicationDbContext _context;
        public PromotionUsedRepo(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> CheckExsit(Guid promotionId, Guid customerId)
        {
           return await _context.PromotionUseds
                  .AnyAsync(p => p.PromotionId == promotionId && p.CustomerId == customerId);
        }

        public async  Task<int> CountPromotionUsedAsync(Guid promotionId)
        {
            return await _context.PromotionUseds
                 .CountAsync(p => p.PromotionId == promotionId);
        }
    }
}
