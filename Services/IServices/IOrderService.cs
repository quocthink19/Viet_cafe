using Repository.Models.DTOs.Request;
using Repository.Models.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.IServices
{
    public interface IOrderService
    {
        Task<OrderResponse> CreateOrder(Guid customerId,OrderRequest order);
        Task DeleteOrder(Guid Id);
        Task<OrderResponse> UpdateOrder(Guid Id, OrderRequest Order);
        Task<IEnumerable<OrderResponse>> GetOrder();
        Task<OrderResponse> GetOrderById(Guid Id);
    }
}
