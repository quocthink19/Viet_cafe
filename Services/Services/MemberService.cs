using Repository.Models;
using Repository.Models.DTOs.Request;
using Repository.UnitOfWork;
using Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class MemberService : IMemberService
    {
        private readonly IUnitOfWork _unitOfWork;
        public MemberService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Member> AddMember(MemberRequest Member)
        {
           var customer = await _unitOfWork.CustomerRepo.GetCustomerById(Member.CustomerId);
            if(customer == null)
            {
                throw new KeyNotFoundException("không tìm thấy khách hàng này");
            }
            Member newMember = new Member {
                Level = Repository.Models.Enum.LevelCus.NEW,
                Point = 0,
                CustomerId = customer.Id,
                Customer = customer,
            }
            ;
            await _unitOfWork.MemberRepo.AddAsync(newMember);
            await _unitOfWork.SaveAsync();
            return newMember;
        }

        public async Task DeleteMember(int Id)
        {
            var member = await GetMemberById(Id);
   //         await _unitOfWork.MemberRepo.DeleteAsync(member);
            await _unitOfWork.SaveAsync();
        }

        public async Task<IEnumerable<Member>> GetMember()
        {
            var members = await _unitOfWork.MemberRepo.GetAllMember();
            return members;
        }

        public async Task<Member> GetMemberByCustomer(Guid CustomerId)
        {
            var member = await _unitOfWork.MemberRepo.GetMemberByCustomerId(CustomerId);
            if (member == null)
            {
                throw new KeyNotFoundException("không tim thấy khách hàng này");
            }
            return member;
        }

        public async Task<Member> GetMemberById(int Id)
        {
            var member = await _unitOfWork.MemberRepo.GetMemberById(Id);
            if(member == null)
            {
                throw new KeyNotFoundException("không tim thấy khách hàng này");
            }
            return member;
        }

        public Task<Member> UpdateMember(int Id, MemberRequest Member)
        {
            return null;
        }
    }
}
