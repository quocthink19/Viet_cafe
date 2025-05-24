using Repository.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Models.DTOs.Response
{
    public class CustomerResponse
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string? FullName { get; set; }
        public DateTime? BirthDate { get; set; }
        public Gender? gender { get; set; }
        public decimal? Wallet { get; set; }
    }
}
