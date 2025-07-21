using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository.IRepository;
using Repository.Models;
using Repository.Models.DTOs.Request;
using Repository.Models.DTOs.Response;
using Services.IServices;
using System.Security.Claims;

namespace Cafe_Web_App.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomerController : Controller
    {
        private readonly ICustomerService _customerService;
        private readonly IMapper _mapper;
        public CustomerController(ICustomerService customerService, IMapper mapper)
        {
            _customerService = customerService;
            _mapper = mapper;
           
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetAll()
        {
            try
            {
                var customers = await _customerService.GetCustomers();

                if (customers != null)
                {
                    var customerResponses = _mapper.Map<IEnumerable<CustomerResponse>>(customers);

                    var response = new TResponse<IEnumerable<CustomerResponse>>(
                        "lấy danh sách khách hàng thành công",
                        customerResponses
                    );
                    return Ok(response);
                }

                return BadRequest("lấy danh sách khách hàng thất bại");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("{Id}")]
        public async Task<ActionResult<Customer>> GetCustomerById(Guid Id)
        {
            try {
                var customer = await _customerService.GetCustomerById(Id);
                if (customer != null)
                {
                    var response = new TResponse<Customer>(
                        "lấy thông tin khách hàng thành công",
                        customer
                        );
                    return Ok(response);
                }
                return BadRequest("lấy danh sách khách hàng thất bại");

            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
       
        [HttpPut("{Id}")]
        public async Task<ActionResult<Customer>> UpdateCustomer(Guid Id,[FromBody] UpdateCustomerRequest customer)
        {
            try
            {
                var updateCustomer = await _customerService.UpdateCustomer(Id,customer);
                if(updateCustomer  != null)
                {
                    var response = new TResponse<Customer>(
                        "cập nhật khách hàng thành công",
                        updateCustomer
                        );
                    return Ok(response);
                }
                return BadRequest("cập nhật khác hàng thất bại");

            }catch (Exception ex) {
            return BadRequest(ex.Message);
            }
        }
       
       
        [Authorize]
        [HttpGet("get-customer")]
        public async Task<ActionResult<CustomerResponse>> GetCustomer() { 
            var customer = await GetCurrentCustomer();
            var res = await _customerService.GetById(customer.Id);
            var respone = new TResponse<CustomerResponse>("lấy thông tin khách hàng thành công", res);
            return Ok(respone);
       }
    private async Task<Customer?> GetCurrentCustomer()
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(username)) return null;
            return await _customerService.GetCustomerByUsername(username);
        }

    }
}
