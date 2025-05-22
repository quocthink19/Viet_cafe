using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Models.DTOs.Request
{
    public class SizeRequest
    {
        public string? Name { get; set; }

        public double? Value { get; set; }

        public double? ExtraPrice { get; set; }
    }
}
