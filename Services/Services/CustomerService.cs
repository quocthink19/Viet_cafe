using AutoMapper;
using Repository.IRepository;
using Repository.Models;
using Repository.Models.DTOs.Request;
using Repository.Models.DTOs.Response;
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
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;

        public CustomerService(IUnitOfWork unitOfWork, IUserService userService, IMapper mapper, IEmailService emailService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _emailService = emailService;
        }
        public async Task<CustomerResponse> TopUpWallet(Guid customerId, double amount)
        {
            var customer = await GetCustomerById(customerId);
            customer.Wallet += (decimal)amount;
            await _unitOfWork.CustomerRepo.Update(customer);
            await _unitOfWork.SaveAsync();
            return _mapper.Map<CustomerResponse>(customer);
        }
        public async Task<CustomerResponse> AddCustomer(AddCustomerRequest customerData)
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
                long mkh = await _unitOfWork.CustomerRepo.GetNextCustomerCodeAsync();
                var newCustomer = new Customer
                {
                    UserId = newUser.Id,
                    MKH = mkh,
                    Verify = false,
                    Wallet = 0,
                    CreatedDate = DateTime.Now
                };

                await _unitOfWork.CustomerRepo.AddAsync(newCustomer);
                await _unitOfWork.SaveAsync();
                var customerResponse = _mapper.Map<CustomerResponse>(newCustomer);

                await SendOTP(newUser.Id, newUser.Email);

                await _unitOfWork.CommitAsync();
                return customerResponse;
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
            var customer = await _unitOfWork.CustomerRepo.GetCustomerById(Id);
            if (customer == null)
                throw new KeyNotFoundException("Không tìm thấy khách hàng này");
            return customer;
        }

        public async Task<CustomerResponse> GetById(Guid Id)
        {
            var customer = await _unitOfWork.CustomerRepo.GetCustomerById(Id);
            if (customer == null)
                throw new KeyNotFoundException("Không tìm thấy khách hàng này");

            return _mapper.Map<CustomerResponse>(customer);
        }

        public async Task<Customer?> GetCustomerByUsername(string username)
        {
            var customer = await _unitOfWork.CustomerRepo.GetCustomerByUsernameAsync(username);
            if (customer == null)
            {
                throw new KeyNotFoundException("Không tìm thấy khách hàng này");
            }
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
                customer.phoneNumber = newCustomer.phoneNumber;

                await _unitOfWork.CustomerRepo.UpdateAsync(customer);

                var user = await _unitOfWork.CustomerRepo.GetUserByCustomerId(Id);
                if (user == null)
                    throw new KeyNotFoundException("Không tìm thấy người dùng");

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

        public async Task<bool> VerifyOTP(string username, string code)
        {
            var user = await _userService.GetUserByUsername(username);
            var otp = await _unitOfWork.OTPCodeRepo.GetValidCodeAsync(user, code);
            if (otp == null || otp.IsUsed || otp.ExpiresAt < DateTime.UtcNow)
            {
                return false;
            }
            otp.IsUsed = true;
            var customer = await _unitOfWork.CustomerRepo.GetCustomerByUserId(user.Id);
            if (customer != null)
            {
                customer.Verify = true;
            }
            await _unitOfWork.SaveAsync();
            return true;
        }
        public async Task SendOTP(Guid userId, string email)
        {
            var otp = new Random().Next(100000, 999999).ToString();

            var OtpCode = new OTPCode
            {
                UserId = userId,
                Code = otp,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMinutes(5),
                IsUsed = false,
            };

            await _unitOfWork.OTPCodeRepo.AddAsync(OtpCode);
            await _unitOfWork.SaveAsync();

            var body = $"Xin chào, mã OTP của bạn là: <b>{otp}</b>. Mã có hiệu lực trong 5 phút.";
            await _emailService.SendEmail(email, "Xác minh tài khoản", body);
        }
    }
}
