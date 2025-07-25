﻿using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public  interface IUserRepo  : IRepository<User>
    {
        Task<User> GetUserByRefreshTokenAsync(string refreshToken);
        Task<User> GetUserByUsernameAsync(string username);
        Task<User> GetByResetTokenAsync(string token);
        Task AddUserAsync(User user);
    }
}
