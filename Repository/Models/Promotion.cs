using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Models
{
    public class Promotion
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Code {  get; set; }
        public double? Condition { get; set; }
        public string? Description { get; set; }
        public double? DiscountPercent { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? CreateDate {  get; set; } = DateTime.Now;
        public DateTime? UpdateDate { get; set; }
    }
}

