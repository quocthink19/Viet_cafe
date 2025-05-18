using Azure;
using Microsoft.AspNetCore.Identity;
using Repository.IRepository;
using Repository.Models;
using Repository.Repositories;
using Repository.UnitOfWork;
using Services.IServices;
using Services.Services;

namespace Cafe_Web_App.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureRepositories(this IServiceCollection services)
        {
            services.AddScoped<IUserRepo, UserRepo>();
            services.AddScoped<ICustomerRepo, CustomerRepo>();
        }
        public static void ConfigureServices(this IServiceCollection services)
        {
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IUserService, UserService>();
        }
        public static void ConfigureUtilities(this IServiceCollection services)
        {
            services.AddScoped<IPasswordHasher<User>,PasswordHasher<User>>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }
    }
}
