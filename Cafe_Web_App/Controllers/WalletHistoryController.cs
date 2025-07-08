using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repository.Models;
using Repository.Models.DTOs.Response;
using Services.IServices;
using Services.Services;
using System.Security.Claims;
using Repository.Models;

namespace Cafe_Web_App.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WalletHistoryController : ControllerBase
    {
        private readonly IWalletHistoryService _service;
        private readonly ICustomerService _customerService;
        public WalletHistoryController(IWalletHistoryService service, ICustomerService customerService)
        {
            _service = service;
            _customerService = customerService;
        }

        [Authorize]
        [HttpGet("get-by-customer")]
        public async Task<ActionResult<IEnumerable<WalletHistory>>> GetByCustomer()
        {
            var customer = await GetCurrentCustomer();
            var histories = await _service.GetWalletHistoryByCustomerId(customer.Id);
            var response = new TResponse<IEnumerable<WalletHistory>>("lấy danh sách thành công", histories);
           return  Ok(response);
        }
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<WalletHistory>> GetById(Guid id)
        {
           
            var history = await _service.GetWalletById(id);
            var response = new TResponse<WalletHistory>("lấy lịch sử thành công", history);
            return Ok(response);
        }
        private async Task<Customer?> GetCurrentCustomer()
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(username)) return null;
            return await _customerService.GetCustomerByUsername(username);
        }
    }
}
