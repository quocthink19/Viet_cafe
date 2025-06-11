using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
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
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly ICustomerService _customerService;

        public UserController(IUserService serService, ICustomerService customerService)
        {
             _userService = serService;
            _customerService = customerService;
        }
        
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequestt loginRequest)
        {
              
            var token = await _userService.LoginAsync(loginRequest.Username, loginRequest.Password);
            var response = new TResponse<AuthResponse>("Đăng nhập thành công",token);
            return Ok(response);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest userData)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var newUser = await _userService.RegisterAsync(userData);

            return Ok(new TResponse<User>("User registered successfully", newUser));
        }
        [HttpPost("register-customer")]
        public async Task<ActionResult<CustomerResponse>> CreateCustomer([FromBody] AddCustomerRequest customer)
        {
            try
            {
                var newCustomer = await _customerService.AddCustomer(customer);
                if (customer != null)
                {
                    var response = new TResponse<CustomerResponse>(
                        "khách hàng đã được tạo thành công",
                        newCustomer
                        );
                    return Ok(response);
                }
                return BadRequest("tạo khách hàng thất bại");

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);

            }
        }
        [HttpPost("verify")]
        public async Task<ActionResult> Verify(string username, string code)
        {
            var check = await _customerService.VerifyOTP(username, code);
            if (!check)
            {
                return BadRequest(new { message = "Xác nhận người dùng thất bại vì mã OTP của bạn sai hoặc đã hết hạn." });
            }
            return Ok(new { message = "xác nhận thành công " });
        }
        [HttpPost("send-otp")]
        public async Task<ActionResult> sendOTP([FromBody] string userName)
        {
            var user = await _userService.GetUserByUsername(userName);

            await _customerService.SendOTP(user.Id, user.Email);
            return Ok("gửi mã OTP thành công cho bạn");
        }

     

        [Authorize]
        [HttpPut("changePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest passwordData)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var usernameClaim = User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(usernameClaim))
            {
                return Unauthorized("Không xác định được người dùng");
            }

            var user = await _userService.GetUserByUsername(usernameClaim);
            var changeSuccess = await _userService.ChangePassword(passwordData, user);

            if (!changeSuccess)
            {
                return BadRequest("Đổi mật khẩu thất bại");
            }

            return Ok("Đổi mật khẩu thành công");
        }
        private async Task<Customer?> GetCurrentCustomer()
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(username)) return null;
            return await _customerService.GetCustomerByUsername(username);
        }
    }
}