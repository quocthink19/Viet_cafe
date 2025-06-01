using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public interface IPromotionRepo : IRepository<Promotion>
    {
        Task<Promotion> GetPromotionByCode(string code);
    }
}
