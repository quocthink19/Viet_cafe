using Microsoft.EntityFrameworkCore;
using Repository.Models;
using Repository.Models.DTOs.Response;
using Repository.Models.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public interface IOrderRepo : IRepository<Order>
    {
        Task<PagedResult<Order>> GetFilteredOrdersAsync(OrderFilter filter);
        Task<IEnumerable<Order>> GetAll();
        Task<Order> GetById(long OrderId);
        Task<bool> ExistsAsync(long id);
        
        Task Delete(long id);
        Task<Customer> GetCustomerByOrderId(long orderId);
        Task<IEnumerable<Order>> GetAllOrdersByCustomerId(Guid customerId);
        Task<int> GetOrdersCountAsync(DateTime start, DateTime end);
        Task<int> GetTotalCupsByPickUpTimeAsync(DateTime start, DateTime end);
        Task<DailyStatsResponse?> GetDailyStatsAsync(DateTime date); 
        Task<MonthlyStatsResponse?> GetMonthlyStatsAsync(int year, int month);
        Task<TodayStatsRessponse> GetTodayStatsAsync();
    }
}
