using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Models.DTOs.Request
{
    public class PaymentRequest
    {
        public long amount { get; set; }
        public string description { get; set; }
        public long orderCode { get; set; }
        public string returnUrl { get; set; }
        public string cancelUrl { get; set; }
        public string signature { get; set; }
    }
}
