using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Models
{
    public class Cart
    {
        public Guid Id { get; set; }

        public double TotalAmount { get; set; }

        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; }
        public List<CartItem> CartItems { get; set; } = new List<CartItem>();

    }
}
