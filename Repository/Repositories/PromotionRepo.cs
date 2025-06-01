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
    public class PromotionRepo : Repository<Promotion>, IPromotionRepo
    {
        ApplicationDbContext _context;
        public PromotionRepo(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Promotion> GetPromotionByCode(string code)
        {
            return await _context.Promotions
                .Where(p => p.Code == code)
                .FirstOrDefaultAsync();
        }
    }
}
