using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Models.Enum
{
    public enum PaymentStatus
    {
        UNPAID = 0,         
        PAID = 1,          
        FAILED = 2,       
        REFUNDED = 3,
    }
}
