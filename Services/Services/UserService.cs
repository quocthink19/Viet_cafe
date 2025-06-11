using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Repository.IRepository;
using Repository.Models;
using Repository.Models.DTOs.Request;
using Repository.Models.DTOs.Response;
using Repository.Models.Enum;
using Repository.UnitOfWork;
using Services.IServices;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IPasswordHasher<User> _passwordHasher;

        public UserService(IUnitOfWork unitOfWork, IConfiguration configuration, IPasswordHasher<User> passwordHasher)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _passwordHasher = passwordHasher;
        }

        public async Task<AuthResponse> LoginAsync(string username, string password)
        {
            var user = await _unitOfWork.UserRepo.GetUserByUsernameAsync(username);
            if (user == null || _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password) != PasswordVerificationResult.Success)
            {
                throw new UnauthorizedAccessException("Tên đăng nhập hoặc mật khẩu không đúng.");
            }
            
            var customer = await _unitOfWork.CustomerRepo.GetCustomerByUsernameAsync(username);
            if(customer != null)
            {
                if (!customer.Verify)
                {
                    throw new UnauthorizedAccessException("vui lòng nhập mã OTP để xác nhận tài khoản");
                }
            }

            var accessToken = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _unitOfWork.UserRepo.UpdateAsync(user);
            await _unitOfWork.SaveAsync();

            return new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                customerId = customer.Id,
                customerName = customer.FullName
            };
        }

        public async Task<User> GetUserByUsername(string username)
        {
            var user = await _unitOfWork.UserRepo.GetUserByUsernameAsync(username);
            if (user == null)
            {
                throw new KeyNotFoundException("Không tìm thấy user.");
            }
            return user;
        }

        public async Task<User> RegisterAsync(RegisterUserRequest newUser)
        {
            var existingUser = await _unitOfWork.UserRepo.GetUserByUsernameAsync(newUser.Username);
            if (existingUser != null)
            {
                throw new InvalidOperationException("Tên đăng nhập đã được sử dụng.");
            }

            var user = new User
            {
                Username = newUser.Username,
                PasswordHash = _passwordHasher.HashPassword(null, newUser.Password),
                Email = newUser.Email,
                RefreshToken = string.Empty,
                RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7),
                role = newUser.role,
                isActive = true
            };

            try
            {
                await _unitOfWork.UserRepo.AddUserAsync(user);
                await _unitOfWork.SaveAsync();
            }
            catch (DbUpdateException ex)
            {
                var errorMessage = ex.InnerException?.Message ?? ex.Message;   
                throw new Exception("Lỗi khi lưu User vào CSDL: " + errorMessage, ex);
            }

            return user;
        }

        public async Task<bool> ChangePassword(ChangePasswordRequest changePassword, User user)
        {
            var checkOldPassword = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, changePassword.OldPassword);
            if (checkOldPassword == PasswordVerificationResult.Failed)
            {
                throw new UnauthorizedAccessException("Mật khẩu hiện tại không chính xác.");
            }
            if (changePassword.NewPassword != changePassword.ConfirmPassword)
            {
                throw new ArgumentException("Mật khẩu mới và mật khẩu xác nhận không khớp.");
            }

            user.PasswordHash = _passwordHasher.HashPassword(user, changePassword.NewPassword);

            await _unitOfWork.UserRepo.UpdateAsync(user);
            await _unitOfWork.SaveAsync();
            return true;
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.role.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        private string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using var rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }
    }
}
