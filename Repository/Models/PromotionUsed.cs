using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Models
{
    public class PromotionUsed
    {
        public Guid PromotionId { get; set; }
        public Guid CustomerId { get; set; }
        public DateTime UsedAt { get; set; }
    }
}
