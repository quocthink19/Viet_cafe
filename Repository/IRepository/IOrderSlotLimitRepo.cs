using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public interface IOrderSlotLimitRepo : IRepository<OrderSlotLimit> 
    {
        Task<OrderSlotLimit> GetSlotByTimeAsync(DateTime? checkTime);
    }
}
