using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Models.DTOs.Request
{
    public class PromotionRequest 
    {
        public string? Name { get; set; }
        public double? Condition { get; set; }
        public string? Description { get; set; }
        public double? DiscountPercent { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
