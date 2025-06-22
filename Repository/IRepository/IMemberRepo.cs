using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public interface IMemberRepo : IRepository<Member>
    {
        Task<IEnumerable<Member>> GetAllMember();
        Task<Member> GetMemberById(int Id);
        Task<Member> GetMemberByCustomerId(Guid CustomerId);
    }
}
