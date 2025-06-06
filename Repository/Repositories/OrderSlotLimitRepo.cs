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
    public class OrderSlotLimitRepo : Repository<OrderSlotLimit>, IOrderSlotLimitRepo
    {
        private readonly ApplicationDbContext _context ;
        public OrderSlotLimitRepo(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<OrderSlotLimit?> GetSlotByTimeAsync(DateTime? checkTime)
        {
            return await _context.OrderSlotLimits
                .FirstOrDefaultAsync(s => s.StartedAt <= checkTime && checkTime <= s.EndTime);
        }
    }
}
