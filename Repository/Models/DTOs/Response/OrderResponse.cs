using Repository.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Models.DTOs.Response
{
    public  class OrderResponse
    {
        public long Id { get; set; }
        public double? TotalAmount { get; set; }
        public string? Code { get; set; }
        public double? DiscountPrice { get; set; }
        public double? FinalPrice { get; set; }
        public string CustomerName { get; set; }
        public string fullName {  get; set; }
        public string? phoneNumber {  get; set; } 
        public string QRcode { get; set; }
        public OrderStatus Status { get; set; }
        public List<OrderItem>? OrderItems { get; set; } = new List<OrderItem>();
    }
}
