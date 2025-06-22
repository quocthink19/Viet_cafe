using Repository.Models;
using Repository.Models.DTOs.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.IServices
{
    public interface MemberService 
    {
        Task<Member> AddMember(MemberRequest Member);
        Task DeleteMember(int Id);
        Task<Member> UpdateMember(int Id, MemberRequest Member);
        Task<IEnumerable<Member>> GetMember();
        Task<Member> GetMemberById(int Id);
    }
}
