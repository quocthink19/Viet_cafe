using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Models
{
    public class OrderSlotLimit
    {
        public Guid Id { get; set; }
        public string SlotName { get; set; }
        public TimeSpan StartTime { get; set; }   // Chỉ lưu giờ bắt đầu
        public TimeSpan EndTime { get; set; }     // Chỉ lưu giờ kết thúc
        public int MaxOrders { get; set; }
        public int MaxCups { get; set; }
    }
}
