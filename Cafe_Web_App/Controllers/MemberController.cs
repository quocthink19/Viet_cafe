using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repository.Models;
using Repository.Models.DTOs.Request;
using Repository.Models.DTOs.Response;
using Services.IServices;
using Services.Services;
using System.Security.Claims;

namespace Cafe_Web_App.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MemberController : ControllerBase
    {
        private readonly IMemberService _memberService;
        private readonly ICustomerService _customerService;
        public MemberController(IMemberService memberService, ICustomerService customerService)
        {
            _memberService = memberService;
            _customerService = customerService;
        }
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<TResponse<IEnumerable<Member>>>> GetMember()
        {
            var members = await _memberService.GetMember();
            var res = new TResponse<IEnumerable<Member>>("lấy danh sách thành viên thành công", members);
            return Ok(res);
        }
        [Authorize]
        [HttpGet("{Id}")]
        public async Task<ActionResult<TResponse<Member>>> GetMemberByID(int Id)
        {
            var member = await _memberService.GetMemberById(Id);
            var res = new TResponse<Member>("lấy thông tin member thành công", member);
            return Ok(res);
        }
        [Authorize]
        [HttpGet("get-member-by-token")]
        public async Task<ActionResult<TResponse<Member>>> GetMemberByToken()
        {
            var customer = await GetCurrentCustomer();
            var member = await _memberService.GetMemberByCustomer(customer.Id);
            var res = new TResponse<Member>("lấy thông tin thành viên thành công", member);
            return Ok(res);
        }
        [Authorize]
        [HttpPost("register-member")]
        public async Task<ActionResult<TResponse<Member>>> RegisterMember([FromBody] MemberRequest member)
        {
            var newMember = await _memberService.AddMember(member);
            var res = new TResponse<Member>("đăng kí thành viên thành công", newMember);
            return Ok(res);
        }
        private async Task<Customer?> GetCurrentCustomer()
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(username)) return null;
            return await _customerService.GetCustomerByUsername(username);
        }
    }
}
