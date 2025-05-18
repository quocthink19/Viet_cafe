using Repository.Models;
using Repository.Models.DTOs.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.IServices
{
    public interface ICustomerService
    {
        Task<Customer> AddCustomer(AddCustomerRequest customerData);
        Task DeleteCustomer(Guid Id);
        Task <Customer> UpdateCustomer(Guid Id,UpdateCustomerRequest newCustomer);
        Task<IEnumerable<Customer>> GetCustomers();
        Task<Customer> GetCustomerById(Guid Id);
    }
}
