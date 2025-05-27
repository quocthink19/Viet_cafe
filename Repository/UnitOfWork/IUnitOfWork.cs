using Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.UnitOfWork
{
    public interface IUnitOfWork
    {
        ICustomerRepo CustomerRepo { get; }
        IUserRepo UserRepo { get; }
        IToppingRepo ToppingRepo { get; }
        IProductRepo ProductRepo { get; }
        ICustomizeToppingRepo CustomizeToppingRepo { get; }
        ICustomizeRepo CustomizeRepo { get; }
        ICartRepo CartRepo { get; }
        IOrderRepo OrderRepo { get; }
        Task<int> SaveAsync();
        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
    }
}
