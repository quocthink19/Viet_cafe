using Repository.Models;
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
        Task<IEnumerable<OrderResponse>> GetAllOrdersByCustomerId(Guid customerId);
        Task DeleteOrder(long Id);
        Task<Customer> GetCustomerByOrderId(long orderId);
        Task<OrderResponse> UpdateOrder(long Id, OrderRequest Order);
        Task<OrderResponse> UpdateOrderByQR(long Id);
        Task<IEnumerable<OrderResponse>> GetOrder();
        Task<OrderResponse> CreateOrderWallet(Guid customerId, OrderRequest order);
        Task<OrderResponse> updateStatusOrder(long Id, StatusOrderRequest status);
        Task<OrderResponse> GetOrderById(long Id);
        Task<string> OrderLimitNotification(DateTime dateTime);


    }
}
