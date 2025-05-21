using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Repository.Models
{
    public class Size
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }

        public double? Value { get; set; }

        public double? ExtraPrice { get; set; }

        [JsonIgnore]
        public List<Customize> Customizes { get; set; } = [];
    }
}
