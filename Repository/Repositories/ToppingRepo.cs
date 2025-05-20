using Repository.IRepository;
using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories
{
    public class ToppingRepo : Repository<Topping> , IToppingRepo
    {
        private readonly ApplicationDbContext _context;
        public ToppingRepo(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
