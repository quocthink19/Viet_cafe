﻿using Microsoft.EntityFrameworkCore.Storage;
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
        public IOrderRepo OrderRepo { get; }
        public ICartRepo CartRepo { get; }
        public ICustomizeRepo CustomizeRepo { get; }
        public ICustomizeToppingRepo CustomizeToppingRepo { get; }


        private IDbContextTransaction _transaction;
        public UnitOfWork(ApplicationDbContext context, ICustomerRepo customerRepo, IUserRepo userRepo,
            ICartRepo cartRepo, IProductRepo productRepo, ICustomizeRepo customizeRepo, ICustomizeToppingRepo customizeToppingRepo)
        {
            _context = context;
            CustomerRepo = customerRepo;
            UserRepo = userRepo;
            CustomizeRepo = customizeRepo;
            CustomizeToppingRepo = customizeToppingRepo;
            ProductRepo = productRepo;
            CartRepo = cartRepo;
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
