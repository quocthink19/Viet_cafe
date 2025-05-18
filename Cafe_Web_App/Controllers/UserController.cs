using Azure;
using Microsoft.AspNetCore.Authorization;
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
        public async Task<ActionResult<string>> Login(string username, string password)
        {
            var token = await _userService.LoginAsync(username, password);
            return Ok(token);
        }
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest userData )
        {
            try
            {
                var newUser = await _userService.RegisterAsync(userData);
                if (newUser != null)
                {
                    var response = new TResponse<User>
                    (
                        "User registered successfully",
                        newUser
                    );

                    return Ok(response);
                }
                return BadRequest(new TResponse<User>("Registration failed", null));
            }
            catch(Exception ex) { 
            return  BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpPut("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest passwordData)
        {
            try
            {
                var usernameClaim = User.FindFirst(ClaimTypes.Name);

                
                var user = await _userService.GetUserByUsername(usernameClaim.Value);
                bool change = await _userService.ChangePassword(passwordData, user);
                if (!change)
                {
                    return BadRequest("đổi mật khẩu thất bại");
                }
                return Ok("đổi mật khẩu thành công");
                
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
