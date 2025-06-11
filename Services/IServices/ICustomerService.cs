using Repository.Models;
using Repository.Models.DTOs.Request;
using Repository.Models.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.IServices
{
    public interface ICustomerService
    {
        Task<CustomerResponse> AddCustomer(AddCustomerRequest customerData);
        Task DeleteCustomer(Guid Id);
        Task <Customer> UpdateCustomer(Guid Id,UpdateCustomerRequest newCustomer);
        Task<IEnumerable<Customer>> GetCustomers();
        Task<Customer> GetCustomerById(Guid Id);
        Task<CustomerResponse> GetById(Guid Id);
        Task<Customer?> GetCustomerByUsername(string username);
        Task<bool> VerifyOTP(string username, string code);

        Task SendOTP(Guid userId, string email);
    }
}
