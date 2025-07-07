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
    public class TopUpRepo : Repository<TopUp>, ITopUpRepo
    {
        private readonly ApplicationDbContext _context;
        public TopUpRepo(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<TopUp> GetTopUpById(long id)
        {
            return await _context.TopUps
                .FirstOrDefaultAsync(t =>t.Id == id);
        }
    }
}
