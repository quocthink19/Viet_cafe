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
    public class OrderRepo : Repository<Order>, IOrderRepo
    {
        private readonly ApplicationDbContext _context;
        public OrderRepo(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        

        public async Task<IEnumerable<Order>> GetAll()
        {
            return await _context.Orders
                .Include(c => c.Customer)
                .Include(o => o.OrderItems)
                .ToListAsync();
        }
        
        public async Task<Order> GetById(long OrderId)
        {
            return await _context.Orders
                .Include(c => c.Customer)
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == OrderId);
        }
        public async Task Delete(long id)
        {
            var entity = await GetById(id);
            if (entity != null)
            {
                _context.Remove(entity);
            }
        }

        public async Task<Customer> GetCustomerByOrderId(long orderId)
        {
            var order = await _context.Orders
                .Include(c => c.Customer)
                    .ThenInclude(u => u.User)
                .Where(o => o.Id == orderId)
                .FirstOrDefaultAsync();

            return order.Customer;
        }

        public async Task<int> GetOrdersCountAsync(DateTime start, DateTime end)
        {
               int totalOrders = await _context.Orders
               .Where(o => o.PickUpTime >= start && o.PickUpTime <= end)
               .CountAsync();
            return totalOrders;
        }
        public async Task<int> GetTotalCupsByPickUpTimeAsync(DateTime start, DateTime end)
        {
            int totalCups = await _context.OrderItems
                .Where(oi => oi.Order.PickUpTime >= start && oi.Order.PickUpTime <= end)
                .SumAsync(oi => oi.Quantity ?? 0);

            return totalCups;
        }

        public async Task<IEnumerable<Order>> GetAllOrdersByCustomerId(Guid customerId)
        {
           return await _context.Orders
                .Include(c => c.Customer)
                .Include(o => o.OrderItems).
                Where(o => o.CustomerId == customerId)
               .ToListAsync();
        }
    }
}
