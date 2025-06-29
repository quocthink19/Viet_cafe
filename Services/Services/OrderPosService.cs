using Repository.Models;
using Repository.Models.DTOs.Request;
using Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class OrderPosService : IOrderPosService
    {
        public Task<OrderPos> CreateOrderPos(OrderPosRequest order)
        {
            throw new NotImplementedException();
        }

        public Task<OrderPos> GetOrderPosById(long Id)
        {
            throw new NotImplementedException();
        }
    }
}
