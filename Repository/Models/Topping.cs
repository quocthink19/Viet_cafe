using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Repository.Models
{
    public class Topping
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public double? Price { get; set; }

        [JsonIgnore]
        public List<CustomizeTopping>? CustomizeToppings { get; set; } = new List<CustomizeTopping>();
    }
}
