using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Repository.IRepository;
using Repository.Models;
using Repository.Models.DTOs.Request;
using Repository.Models.Enum;
using Repository.UnitOfWork;
using Services.IServices;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
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
        public async Task<string> LoginAsync(string username, string password)
        {
            var user = await _unitOfWork.UserRepo.GetUserByUsernameAsync(username);
            if(user == null || _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password) != PasswordVerificationResult.Success){
                throw new Exception("Invalid usernamr or password");
            }
            return generateJwtToken(user);
        }
        public async Task<User> GetUserByUsername(string username)
        {
            var user = await _unitOfWork.UserRepo.GetUserByUsernameAsync(username);
            if (user == null)
            {
                throw new Exception("không tim thấy user");
            }
            return user;
        }

        public async  Task<User> RegisterAsync(RegisterUserRequest newUser)
        {
            var existingUser = await _unitOfWork.UserRepo.GetUserByUsernameAsync(newUser.Username);
            if (existingUser != null)
            {
                throw new InvalidOperationException("Username already taken");
            }

            var user = new User
            {
                Username = newUser.Username,
                PasswordHash = _passwordHasher.HashPassword(null, newUser.Password),
                Email = newUser.Email,
                role = newUser.role
            };

            await _unitOfWork.UserRepo.AddUserAsync(user);
            await _unitOfWork.SaveAsync();
            return user;
        }

        public async Task<bool> ChangePassword(ChangePasswordRequest changePassword, User user)
        {
            var checkOldPasswrd =  _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, changePassword.OldPassword);
            if(checkOldPasswrd == PasswordVerificationResult.Failed)
            {
                throw new Exception("mật khẩu hiện tại không chính xác");
            }
            if(changePassword.NewPassword != changePassword.ConfirmPassword ) {
                throw new Exception("mật khẩu mới và mật khẩu xác nhận không khớp ");
            }

            user.PasswordHash = _passwordHasher.HashPassword(user, changePassword.NewPassword);

            await _unitOfWork.UserRepo.UpdateAsync(user);
            await _unitOfWork.SaveAsync();
            return true;
        }

        public string generateJwtToken(User user)
        {
            var claims = new[]
            {
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
    }
}
