using Repository.Models;
using Repository.Models.DTOs.Request;
using Repository.Models.DTOs.Response;
using Repository.UnitOfWork;
using Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        public OrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<OrderResponse> CreateOrder(Guid customerId,OrderRequest order)
        {
            /*var cart = await _unitOfWork.CartRepo.GetCartByCustomerId(customerId);

            var order = new Order
            {
                TotalAmount = cart.TotalAmount
               ,
            };*/
            return null;
        }

        public Task DeleteOrder(Guid Id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<OrderResponse>> GetOrder()
        {
            throw new NotImplementedException();
        }

        public Task<OrderResponse> GetOrderById(Guid Id)
        {
            throw new NotImplementedException();
        }

        public Task<OrderResponse> UpdateOrder(Guid Id, OrderRequest Order)
        {
            throw new NotImplementedException();
        }
    }
}
