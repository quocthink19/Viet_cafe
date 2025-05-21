using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Repository.Models
{
    public class Product
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public double? Price { get; set; }
        public double? Rating { get; set; }
        public double? PurchaseCount { get; set; }
        public bool IsAvaillable { get; set; }

        public Guid CategoryId { get; set; }
        public Category Category { get; set; }
        [JsonIgnore]
        public List<Customize> Customizes { get; set; } = new List<Customize>();
    }
}
