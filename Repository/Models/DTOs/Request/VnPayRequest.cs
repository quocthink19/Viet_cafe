using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Models.DTOs.Request
{
    public class VnPayRequest
    {
        public long OrderId { get; set; }   
        public double Amount { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CustomerId { get; set; }
    }
}
