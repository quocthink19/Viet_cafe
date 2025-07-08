using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Repository.Models
{
    public class WalletHistory
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }

     
        public DateTime TransactionDate { get; set; }
        [Precision(18, 6)]
        public decimal AmountChanged { get; set; }
        public string? Description { get; set; }
        [Precision(18, 6)]
        public decimal RemainingAmount { get; set; }

        [JsonIgnore]
        public virtual Customer Customer { get; set; }
    }
}
