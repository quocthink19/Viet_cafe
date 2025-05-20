using Repository.IRepository;
using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories
{
    public class SizeRepo : Repository<Size>, ISizeRepo
    {
        private readonly ApplicationDbContext _scontext;
        public SizeRepo(ApplicationDbContext context) : base(context)
        {
            _scontext = context;
        }
    }
}
