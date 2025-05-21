using Repository.IRepository;
using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories
{
    public class CustomizeToppingRepo : Repository<CustomizeTopping>, ICustomizeToppingRepo
    {
        private readonly ApplicationDbContext _context;
        public CustomizeToppingRepo(ApplicationDbContext context) : base(context)
        {
            _context = context;   
        }
    }
}
