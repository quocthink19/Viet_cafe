using Repository.Models;
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
        Task<IEnumerable<Order>> GetAll();
        Task<Order> GetById(Guid OrderId);
        Task<Customer> GetCustomerByOrderId(Guid orderId);
        Task<int> GetOrdersCountAsync(DateTime start, DateTime end);
        Task<int> GetTotalCupsByPickUpTimeAsync(DateTime start, DateTime end);
    }
}
