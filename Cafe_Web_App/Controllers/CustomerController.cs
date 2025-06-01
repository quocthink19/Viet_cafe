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
        private readonly IUserService _userService;
        public CustomerController(ICustomerService customerService, IUserService userService)
        {
            _customerService = customerService;
            _userService = userService;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetAll()
        {
            try
            {
                var customers = await _customerService.GetCustomers();
                if (customers != null)
                {
                    var response = new TResponse<IEnumerable<Customer>>
                    (
                        "lấy danh sách khách hàng thành công",
                        customers
                    );
                    return Ok(response);
                }
                return BadRequest("lấy danh sách khách hàng thất bại");
            }catch(Exception ex)
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

            } catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("send-otp")]
        public async Task<ActionResult> sendOTP([FromBody]string userName)
        {
           var user = await _userService.GetUserByUsername(userName);

            await _customerService.SendOTP(user.Id, user.Email);
            return Ok("gửi mã OTP thành công cho bạn");
        }

        [HttpPost]
        public async Task<ActionResult<CustomerResponse>> CreateCustomer([FromBody ] AddCustomerRequest customer)
        {
            try
            {
                var newCustomer = await _customerService.AddCustomer(customer);
                if(customer != null)
                {
                    var response = new TResponse<CustomerResponse>(
                        "khách hàng đã được tạo thành công",
                        newCustomer
                        );
                    return Ok(response);
                }
                return BadRequest("tạo khách hàng thất bại");

            }catch(Exception ex ) {
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
       
        [HttpPost("verify")]
        public async Task<ActionResult> Verify(string username, string code)
        {
            var check = await _customerService.VerifyOTP(username, code);
            if(!check)
            {
                return BadRequest(new { message = "Xác nhận người dùng thất bại vì mã OTP của bạn sai hoặc đã hết hạn." });
            }
            return Ok(new { message = "xác nhận thành công " });
        }
        private async Task<Customer?> GetCurrentCustomer()
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(username)) return null;
            return await _customerService.GetCustomerByUsername(username);
        }

    }
}
