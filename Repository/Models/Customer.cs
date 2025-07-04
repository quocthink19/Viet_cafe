using Repository.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Repository.Models
{
    public class Customer
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public long MKH {get; set; }
        public string? FullName { get; set; }
        public DateTime? BirthDate { get; set; }
        public Gender? gender { get; set; }
        public string? phoneNumber { get; set; }
        public decimal? Wallet { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool Verify { get; set; }
        [JsonIgnore]
        public Cart? Cart { get; set; }

        [JsonIgnore]
        public virtual Member Member { get; set; }

        [JsonIgnore]
        public virtual User? User { get; set; }
    }
}
