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
    public class CartRepo : Repository<Cart>, ICartRepo
    {
        private readonly ApplicationDbContext _context;
        public CartRepo(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Cart?> GetCartByCustomerId(Guid customerId)
        {
            return await _context.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Customize)
                        .ThenInclude(cz => cz.Size) 
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Customize)
                        .ThenInclude(cz => cz.Product) 
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Customize)
                        .ThenInclude(cz => cz.CustomizeToppings)
                            .ThenInclude(ct => ct.Topping) 
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);
        }
        public async Task<Cart?> GetCartById(Guid Id)
        {
            return await _context.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Customize)
                        .ThenInclude(cz => cz.Size)
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Customize)
                        .ThenInclude(cz => cz.Product)
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Customize)
                        .ThenInclude(cz => cz.CustomizeToppings)
                            .ThenInclude(ct => ct.Topping)
                .FirstOrDefaultAsync(c => c.Id == Id);
        }

        public async Task<CartItem> GetCartItem(Guid CartId, Guid CustomizeId)
        {
            return await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.CartId == CartId && ci.CustomizeId == CustomizeId);
        }
        public async Task DeleteCartItem(CartItem item)
        {
            _context.CartItems.Remove(item);
            await _context.SaveChangesAsync();
        }
        public async Task InsertCartItemAsync(CartItem item)
        {
            await _context.CartItems.AddAsync(item);
        }

        public async Task<double?> CalculateTotalAmount(Guid cartId)
        {
            return await _context.CartItems
                .Where(ci => ci.CartId == cartId)
                .SumAsync(ci => ci.UnitPrice * ci.Quantity);
        }

        public async Task DeleteAllCartItems(Guid cartId)
        {
            var items = _context.CartItems.Where(ci => ci.CartId == cartId);
            _context.CartItems.RemoveRange(items);
        }
    }
}
