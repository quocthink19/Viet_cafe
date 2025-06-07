using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Models.DTOs.Response
{
    public class CartResponse
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
     // public CustomerResponse? Customer { get; set; }
        public double? TotalAmount { get; set; }
        public List<CartItemResponse>? CartItems { get; set; } = new();


    }
}
