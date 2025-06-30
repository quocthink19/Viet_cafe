using Repository.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Models.Filter
{
    public class OrderFilter
    {
        public string? Code { get; set; }
        public OrderStatus? Status { get; set; }
        public Method? Payment { get; set; }
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public Guid? CustomerId { get; set; }

        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
