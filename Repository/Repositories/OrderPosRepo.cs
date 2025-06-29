using Repository.IRepository;
using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories
{
    public class OrderPosRepo : Repository<OrderPos>, IOrderPosRepo
    {
        public OrderPosRepo(ApplicationDbContext context) : base(context)
        {
        }
    }
}
