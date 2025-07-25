﻿using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public interface ICustomerRepo : IRepository<Customer>
    {
        Task<User> GetUserByCustomerId(Guid customerId);
        Task<IEnumerable<Customer>> GetAllCus();
        Task<long> GetNextCustomerCodeAsync();
        Task<Customer> GetCustomerById(Guid customerId);
        Task<Customer> GetCustomerByUserId(Guid userId);
        Task<Customer?> GetCustomerByUsernameAsync(string username);
    }
}
