using Repository.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Models
{
    public class OrderPos
    {
        public long Id { get; set; }
        public string? Code { get; set; }
        public Guid TableId { get; set; } 
        public DateTime CreatedAt { get; set; }
        public Method Payment {  get; set; }
        public List<OrderItem> Items { get; set; }       
        public Table Table { get; set; }
        public double? FinalPrice { get; set; }
        public DateTime CreateAt { get; set; }
    }
}
