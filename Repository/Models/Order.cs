﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repository.Models.Enum;

namespace Repository.Models
{
    public  class Order
    {
        public long Id { get; set; }
        public string? Code { get; set; }
        public double? TotalAmount { get; set; }
        public double? DiscountPrice { get; set; }
        public double? FinalPrice { get; set; }
        public DateTime? PickUpTime { get; set; }
        public OrderStatus Status { get; set; }
        public string? fullName { get; set; }
        public string? phoneNumber { get; set; }
        public string QRcode { get; set; }
        public Method? Payment { get; set; }
        public DateTime CreateAt { get; set; }
        public Guid CustomerId { get; set; }
        public Customer? Customer { get; set; }
        public List<OrderItem>? OrderItems { get; set; } = new List<OrderItem>();
        public List<Payment>? Payments { get; set; } = new List<Payment>();
    }
}
