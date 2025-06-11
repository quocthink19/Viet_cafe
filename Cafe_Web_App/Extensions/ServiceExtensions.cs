using Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using Repository.IRepository;
using Repository.Models;
using Repository.Repositories;
using Repository.UnitOfWork;
using System.Reflection;
using Services.Services;
using Services.IServices;
using Net.payOS;
using Repository.Helper;

namespace Cafe_Web_App.Extensions
{
    public static class ServiceExtensions
    { 
           public static void AddRepoBase(this IServiceCollection services)
    {
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
    }

    public static void ConfigureRepositories(this IServiceCollection services)
    {
        services.Scan(scan => scan
            .FromAssemblies(Assembly.Load("Repository"))
            .AddClasses(classes => classes
                .Where(type => type.Namespace == "Repository.Repositories" && type.Name.EndsWith("Repo")))
            .AsImplementedInterfaces()
            .WithScopedLifetime());
    }

        public static void ConfigureServices(this IServiceCollection services)
        {

            services.Scan(scan => scan
            .FromAssemblies(Assembly.Load("Services"))
            .AddClasses(classes => classes
                .Where(type => type.Namespace == "Services.Services" && type.Name.EndsWith("Service")))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        }
    public static void ConfigureUtilities(this IServiceCollection services)
    {
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
    }
}
}
  /*  {
        public static void ConfigureRepositories(this IServiceCollection services)
        {
            services.AddScoped<IUserRepo, UserRepo>();
            services.AddScoped<ICustomerRepo, CustomerRepo>();
            services.AddScoped<ICategoryRepo, CategoryRepo>();
            services.AddScoped<IToppingRepo, ToppingRepo>();
        }
        public static void ConfigureServices(this IServiceCollection services)
        {
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IToppingService, ToppingService>();
        }
        public static void ConfigureUtilities(this IServiceCollection services)
        {
            services.AddScoped<IPasswordHasher<User>,PasswordHasher<User>>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }
    }*/
//}

