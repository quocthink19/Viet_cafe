﻿using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public interface IPaymentRepo : IRepository<Payment>
    {
        Task<Payment> GetByOrderId(long orderId);
    }
}
