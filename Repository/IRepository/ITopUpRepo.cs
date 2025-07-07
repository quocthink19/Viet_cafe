using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public interface ITopUpRepo : IRepository<TopUp>
    {
        Task<TopUp> GetTopUpById(long id);
    }
}
