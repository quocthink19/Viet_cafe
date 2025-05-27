using Repository.Models;
using Repository.Models.DTOs.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public interface ICustomizeRepo : IRepository<Customize>
    {
        Task<Customize?> GetById(Guid id);
        Task<IEnumerable<Customize>> GetAll();
        IQueryable<Customize> GetQueryable(); 
    }
}
