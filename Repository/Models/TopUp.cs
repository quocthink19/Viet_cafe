using Repository.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Models
{
    public class TopUp
    {
        public long Id { get; set; } 
        public Guid CustomerId { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; }
        public PaymentStatus Status { get; set; } 
        public string Description { get; set; }
        public decimal Wallet {  get; set; }
        public string? TransactionIdResponse { get; set; }
    }
}