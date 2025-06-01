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
    public class OTPCodeRepo : Repository<OTPCode>, IOTPCodeRepo
    {
        private readonly ApplicationDbContext _context ;
        public OTPCodeRepo(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<OTPCode> GetValidCodeAsync(User user, string code)
        {
            return await _context.OTPCodes
                .Where(o => o.UserId == user.Id &&
                        o.Code == code &&
                        !o.IsUsed &&
                        o.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(o => o.CreatedAt)
            .FirstOrDefaultAsync();
        }
    }
}
