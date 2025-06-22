using Repository.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Models
{
    public class Member
    {
        public int Id { get; set; }
        public int Point { get; set; }
        public LevelCus Level { get; set; }
       
        public Guid CustomerId {  get; set; }
        public virtual Customer Customer { get; set; }
    }
}
