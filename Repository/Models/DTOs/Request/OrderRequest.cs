using Repository.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Models.DTOs.Request
{
    public class OrderRequest
    {
        public string? Code { get; set; }
        public DateTime? PickUpTime { get; set; }
        public Method? Paymemt { get; set; }
    }
}
