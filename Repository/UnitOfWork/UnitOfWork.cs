using Microsoft.EntityFrameworkCore.Storage;
using Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public ICustomerRepo CustomerRepo { get; }
        public IUserRepo UserRepo { get; }
        public IToppingRepo ToppingRepo { get; }
        public IProductRepo ProductRepo { get; }
        public IOrderPosRepo OrderPosRepo { get; }
        public IOrderRepo OrderRepo { get; }
        public IOrderSlotLimitRepo OrderSlotLimitRepo { get; }
        public ICartRepo CartRepo { get; }
        public ICustomizeRepo CustomizeRepo { get; }
        public ICustomizeToppingRepo CustomizeToppingRepo { get; }
        public IPaymentRepo PaymentRepo { get; }
        public IOTPCodeRepo OTPCodeRepo { get; }
        public IMemberRepo MemberRepo { get; }
        public ITopUpRepo TopUpRepo { get; }
        public IWalletHistoryRepo WalletHistoryRepo { get; }


        private IDbContextTransaction _transaction;
        public UnitOfWork(ApplicationDbContext context, ICustomerRepo customerRepo, IUserRepo userRepo,
            ICartRepo cartRepo, IProductRepo productRepo, ICustomizeRepo customizeRepo, ICustomizeToppingRepo customizeToppingRepo,
            IOTPCodeRepo oTPCodeRepo, IOrderRepo orderRepo, IOrderSlotLimitRepo orderSlotLimitRepo, IPaymentRepo paymentRepo,
            IMemberRepo memberRepo, IOrderPosRepo orderPosRepo, ITopUpRepo topUpRepo, IWalletHistoryRepo walletHistoryRepo)
        {
            _context = context;
            CustomerRepo = customerRepo;
            UserRepo = userRepo;
            CustomizeRepo = customizeRepo;
            CustomizeToppingRepo = customizeToppingRepo;
            ProductRepo = productRepo;
            CartRepo = cartRepo;
            OTPCodeRepo = oTPCodeRepo;
            OrderRepo = orderRepo;
            OrderSlotLimitRepo = orderSlotLimitRepo;
            PaymentRepo = paymentRepo;
            MemberRepo = memberRepo;
            OrderPosRepo = orderPosRepo;
            TopUpRepo = topUpRepo;
            WalletHistoryRepo = walletHistoryRepo;
        }

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }
        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }
}
