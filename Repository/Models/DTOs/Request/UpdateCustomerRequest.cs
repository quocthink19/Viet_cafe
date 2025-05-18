using Repository.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Models.DTOs.Request
{
    public class UpdateCustomerRequest
    {
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public DateTime BirthDate { get; set; }
        public Gender gender { get; set; }
    }
}
