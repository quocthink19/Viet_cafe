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
    public class MemberRepo : Repository<Member>, IMemberRepo
    {
        private readonly ApplicationDbContext _context;
        public MemberRepo(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Member>> GetAllMember()
        {
            return await _context.Members
                .Include(c => c.Customer)
                .ToListAsync();
        }

        public async  Task<Member> GetMemberByCustomerId(Guid customerId)
        {
            return await _context.Members
                .Include(c => c.Customer)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);
        }

        public async Task<Member> GetMemberById(int id)
        {
            return await _context.Members
              .Include(c => c.Customer)
              .FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}
