﻿using Microsoft.EntityFrameworkCore;
using Repository.IRepository;
using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories
{
    public class PaymentRepo : Repository<Payment>, IPaymentRepo
    {
        private readonly ApplicationDbContext _context;
        public PaymentRepo(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Payment> GetByOrderId(long orderId)
        {
            return await _context.payments
                 .Include(o => o.Order)
                 .FirstOrDefaultAsync(x => x.OrderId == orderId);
        }
    }
}
