using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Repository.Models;
using Repository.Models.DTOs.Request;
using Repository.Models.DTOs.Response;
using Services.IServices;
using System.Security.Claims;

namespace Cafe_Web_App.Controllers
{
   
    [ApiController]
    [Route("[controller]")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService serService)
        {
             _userService = serService;
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
    }
}