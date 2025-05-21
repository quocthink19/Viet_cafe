using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Models.DTOs.Response
{
    public class ProductResponse
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public double? Price { get; set; }
        public double? Rating { get; set; }
        public double? PurchaseCount { get; set; }
        public bool IsAvaillable { get; set; }

        public Guid CategoryId { get; set; }
    }
}
