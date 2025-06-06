using Repository.Models;
using Repository.Models.DTOs.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.IServices
{
    public interface IOrderSlotLimitService
    {
        Task<OrderSlotLimit> AddOrderSlotLimit(OrderSlotLimitRequest OrderSlotLimit);
        Task DeleteOrderSlotLimit(Guid Id);
        Task<OrderSlotLimit> UpdateOrderSlotLimit(Guid Id, OrderSlotLimitRequest OrderSlotLimit);
        Task<IEnumerable<OrderSlotLimit>> GetOrderSlotLimit();
        Task<OrderSlotLimit> GetOrderSlotLimitById(Guid Id);
    }
}
