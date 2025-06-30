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
    public class ProductRepo : Repository<Product>, IProductRepo
    {
        private readonly ApplicationDbContext _context;
        public ProductRepo(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Product>> GetAll()
        {
            return await _context.Products
                .Include(p => p.Category)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetBestSeller()
        {
            return await _context.Products
            .OrderByDescending(p => p.PurchaseCount)
            .Take(4)
            .ToListAsync();
        }

        public async Task<Product> GetById(Guid id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .FirstAsync(p => p.Id == id);
        }

    }
}
