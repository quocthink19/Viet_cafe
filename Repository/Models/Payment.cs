using Repository.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Models
{
    public class Payment
    {
        public string? Id { get; set; }
        public string? Code { get; set; }
        public double? Amount { get; set; }
        public int? Method { get; set; }

        public long? OrderId { get; set; }
        public long? MKH {get; set; }

        public string? Description { get; set; }

        public PaymentStatus? Status { get; set; }

        public string? TransactionIdResponse { get; set; }

        public virtual Order Order { get; set; }
    }
}
