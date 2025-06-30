using Microsoft.EntityFrameworkCore;
using Repository.IRepository;
using Repository.Models;
using Repository.Models.DTOs.Response;
using Repository.Models.Enum;
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
           .Include(o => o.OrderItems)
           .Where(o => o.CustomerId == customerId)
           .OrderByDescending(o => o.CreateAt) 
           .ToListAsync();
        }

        public async Task<DailyStatsResponse?> GetDailyStatsAsync(DateTime date)
        {
            var stats = await _context.Orders
        .Where(o => o.Status == OrderStatus.COMPLETED && o.PickUpTime.HasValue && o.PickUpTime.Value.Date == date.Date)
        .GroupBy(o => o.PickUpTime.Value.Date)
        .Select(g => new DailyStatsResponse
        {
            Date = g.Key,
            TotalOrders = g.Count(),
            TotalRevenue = g.Sum(o => o.FinalPrice ?? 0)
        })
        .FirstOrDefaultAsync();

            return stats;
        }

        public async Task<MonthlyStatsResponse?> GetMonthlyStatsAsync(int year, int month)
        {
            var stats = await _context.Orders
              .Where(o =>
                  o.Status == OrderStatus.COMPLETED &&
                  o.PickUpTime.HasValue &&
                  o.PickUpTime.Value.Year == year &&
                  o.PickUpTime.Value.Month == month)
              .GroupBy(o => new { o.PickUpTime.Value.Year, o.PickUpTime.Value.Month })
              .Select(g => new MonthlyStatsResponse
              {
                  Year = g.Key.Year,
                  Month = g.Key.Month,
                  TotalOrders = g.Count(),
                  TotalRevenue = g.Sum(o => o.FinalPrice ?? 0)
              })
              .FirstOrDefaultAsync();

            return stats;
        }

        public async Task<TodayStatsRessponse> GetTodayStatsAsync()
        {
            var today = DateTime.Today;

            var stats = await _context.Orders
                .Where(o => o.Status == OrderStatus.COMPLETED && o.PickUpTime.HasValue && o.PickUpTime.Value.Date == today)
                .GroupBy(o => o.PickUpTime.Value.Date)
                .Select(g => new TodayStatsRessponse
                {
                    Date = g.Key,
                    TotalOrders = g.Count(),
                    TotalRevenue = g.Sum(o => o.FinalPrice ?? 0)
                })
                .FirstOrDefaultAsync();

            // Nếu không có đơn nào hôm nay thì trả về 0
            return stats ?? new TodayStatsRessponse
            {
                Date = today,
                TotalOrders = 0,
                TotalRevenue = 0
            };
        }
    }
}
