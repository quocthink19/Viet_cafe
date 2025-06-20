using Repository.Models;
using Repository.Models.DTOs.Request;
using Repository.Models.DTOs.Response;
using Repository.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.IServices
{
    public interface IUserService
    {
        Task <AuthResponse> LoginAsync(string username, string password);
        Task<AuthResponse> RefreshTokenAsync(string refreshToken);
        Task <User>RegisterAsync (RegisterUserRequest user);
        Task <User> GetUserByUsername(string userName);
        Task <bool> ChangePassword(ChangePasswordRequest changePassword, User user); 
    }

}
