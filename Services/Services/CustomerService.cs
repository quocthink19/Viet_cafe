using Repository.IRepository;
using Repository.Models;
using Repository.Models.DTOs.Request;
using Repository.UnitOfWork;
using Services.IServices;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;

        public CustomerService(IUnitOfWork unitOfWork, IUserService userService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public async Task<Customer> AddCustomer(AddCustomerRequest customerData)
        {
            if (customerData == null)
                throw new ArgumentNullException(nameof(customerData));

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var userData = new RegisterUserRequest
                {
                    Username = customerData.Username,
                    Email = customerData.Email,
                    Password = customerData.Password,
                    role = Repository.Models.Enum.UserRole.CUSTOMER
                };

                var newUser = await _userService.RegisterAsync(userData);

                var newCustomer = new Customer
                {
                    UserId = newUser.Id,
                    BirthDate = customerData.BirthDate,
                    FullName = customerData.FullName,
                    gender = customerData.gender,
                    Wallet = 0,
                    CreatedDate = DateTime.Now
                };

                await _unitOfWork.CustomerRepo.AddAsync(newCustomer);
                await _unitOfWork.SaveAsync();

                await _unitOfWork.CommitAsync();
                return newCustomer;
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task DeleteCustomer(Guid Id)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var customer = await GetCustomerById(Id);

                await _unitOfWork.CustomerRepo.DeleteAsync(Id);
                await _unitOfWork.SaveAsync();

                await _unitOfWork.CommitAsync();
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<Customer> GetCustomerById(Guid Id)
        {
            var customer = await _unitOfWork.CustomerRepo.GetByIdAsync(Id);
            if (customer == null)
                throw new Exception("Không tìm thấy customer này");

            return customer;
        }

        public async Task<IEnumerable<Customer>> GetCustomers()
        {
            var customers = await _unitOfWork.CustomerRepo.GetAllAsync();
            return customers;
        }

        public async Task<Customer> UpdateCustomer(Guid Id, UpdateCustomerRequest newCustomer)
        {
            if (newCustomer == null)
                throw new ArgumentNullException(nameof(newCustomer));

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var customer = await GetCustomerById(Id);
                customer.FullName = newCustomer.FullName;
                customer.BirthDate = newCustomer.BirthDate;
                customer.gender = newCustomer.gender;

                await _unitOfWork.CustomerRepo.UpdateAsync(customer);

                var user = await _unitOfWork.CustomerRepo.GetUserByCustomerId(Id);
                if (user == null)
                    throw new Exception("Không tìm thấy user");

                user.Email = newCustomer.Email;
                await _unitOfWork.UserRepo.UpdateAsync(user);

                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitAsync();

                return customer;
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }
    }
}
