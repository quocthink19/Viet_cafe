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
    public class CustomizeRepo : Repository<Customize>, ICustomizeRepo
    {
        private readonly ApplicationDbContext _context;
        public CustomizeRepo(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Customize>> GetAll()
        {
            return await _context.Customizes
            .Include(c => c.Size)
            .Include(c => c.Product)
            .Include(c => c.CustomizeToppings)
                .ThenInclude(ct => ct.Topping)
            .ToListAsync();
        }
        public async Task<Customize?> GetById(Guid id)
        {
            return await _context.Customizes
                .Include(c => c.Size)
                .Include(c => c.Product)
                .Include(c => c.CustomizeToppings)
                    .ThenInclude(ct => ct.Topping)
                .FirstOrDefaultAsync(c => c.Id == id);
        }
    }
   }

