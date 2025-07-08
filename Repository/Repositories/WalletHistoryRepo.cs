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
    public class WalletHistoryRepo : Repository<WalletHistory>, IWalletHistoryRepo
    {
        private readonly ApplicationDbContext _context;
        public WalletHistoryRepo(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<WalletHistory>> GetByCustomerID(Guid Id)
        {
                   return await _context.WalletHistories
                  .Where(c => c.CustomerId == Id)
                  .OrderByDescending(c => c.TransactionDate)
                  .ToListAsync();
        }
    }
}
