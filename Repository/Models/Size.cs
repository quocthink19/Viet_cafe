using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Models
{
    public class Size
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }

        public double? Volume { get; set; }
    }
}
