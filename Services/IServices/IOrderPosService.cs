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
    public interface IOrderPosService
    {
        public Task<OrderPos> CreateOrderPos(OrderPosRequest order);
        Task<OrderPos> GetOrderPosById(long Id);

    }
}
