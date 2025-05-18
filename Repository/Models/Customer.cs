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
        public string? FullName { get; set; }
        public DateTime BirthDate { get; set; }
        public Gender gender { get; set; }
        public decimal Wallet { get; set; }
        public int Point {  get; set; }
        public DateTime CreatedDate { get; set; }

        [JsonIgnore]
        public virtual User? User { get; set; }
    }
}
