using Microsoft.EntityFrameworkCore;
using Repository.IRepository;
using Repository.Models;
using Repository.Models.Filter;
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
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<PagedResult<Product>> GetAllFilteredProductsAsync(ProductFilter filter)
        {
            var query = _context.Products.
                Include(c => c.Category).
                AsQueryable()
                .Where(m => m.IsAvaillable == true);

            if (!string.IsNullOrEmpty(filter.Name))
                query = query.Where(p => p.Name.Contains(filter.Name));

            if (filter.CategoryId.HasValue)
                query = query.Where(p => p.CategoryId == filter.CategoryId.Value);

            if (filter.MinPrice.HasValue)
                query = query.Where(p => p.Price >= filter.MinPrice.Value);

            if (filter.MaxPrice.HasValue)
                query = query.Where(p => p.Price <= filter.MaxPrice.Value);

            if (filter.IsAvailable.HasValue)
                query = query.Where(p => p.IsAvaillable == filter.IsAvailable.Value);

            if (filter.MinRating.HasValue)
                query = query.Where(p => p.Rating >= filter.MinRating.Value);

            // Pagination
            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)filter.PageSize);
            var skip = (filter.Page - 1) * filter.PageSize;

            var items = (await query
            .Skip(skip)
            .Take(filter.PageSize)
            .ToListAsync()).AsEnumerable()
            .Reverse()
            .ToList();

            return new PagedResult<Product>
            {
                CurrentPage = filter.Page,
                TotalPages = totalPages,
                PageSize = filter.PageSize,
                TotalCount = totalCount,
                Items = items
            };
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

        public async Task<Guid?> GetProductIdByCartItemIdAsync(Guid cartItemId)
        {
            var cartItem = await _context.CartItems
               .Include(ci => ci.Customize)
               .FirstOrDefaultAsync(ci => ci.Id == cartItemId);

            return cartItem?.Customize?.ProductId;
        }

        public Task updatePurchaseCount(Guid Id, int quanity)
        {
            throw new NotImplementedException();
        }
    }
}
