using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Models.DTOs.Request
{
    public class OrderSlotLimitRequest
    {
        public string SlotName { get; set; }
        public int MaxOrders { get; set; }
        public int MaxCups { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime EndTime { get; set; }
    }
}
