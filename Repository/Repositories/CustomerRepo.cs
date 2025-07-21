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
    public class CustomerRepo : Repository<Customer>, ICustomerRepo
    {
        private readonly ApplicationDbContext _context;
        public CustomerRepo(ApplicationDbContext context) : base(context) {
            _context = context;
        }

        public async Task<Customer> GetCustomerById(Guid customerId)
        {
            return await _context.Customers
                 .Include(u => u.User)
                 .Where(c => c.Id == customerId)
                 .FirstOrDefaultAsync();
        }

        public async Task<Customer> GetCustomerByUserId(Guid userId)
        {
            return await _context.Customers
                 .Where(x => x.UserId == userId)
                 .FirstOrDefaultAsync();
        }

        public async Task<long> GetNextCustomerCodeAsync()
        {
            var lastCustomer = await _context.Customers
                .OrderByDescending(c => c.MKH)
                .FirstOrDefaultAsync();

            return (lastCustomer?.MKH ?? 0) + 1;
        }

        public async  Task<Customer?> GetCustomerByUsernameAsync(string username)
        {
         return await _context.Customers
        .Include(c => c.User)
        .Where(c => c.User != null && c.User.Username == username || c.User.Email == username)
        .FirstOrDefaultAsync();
        }

        public async Task<User> GetUserByCustomerId(Guid customerId)
        {
            var user = await _context.Customers.
               Where(c => c.Id == customerId)
               .Select(c => c.User)
               .FirstOrDefaultAsync();

            return user;
        }

        public async Task<IEnumerable<Customer>> GetAllCus()
        {
           return await _context.Customers
                .Include (c => c.User)
                .ToArrayAsync();
        }
    }
}
