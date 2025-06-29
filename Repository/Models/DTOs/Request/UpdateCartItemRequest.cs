using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Models.DTOs.Request
{
    public class UpdateCartItemRequest
    {
        public Guid CartItemId { get; set; }
        public int newQuantity { get; set; }
    }
}
