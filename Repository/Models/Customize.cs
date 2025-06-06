using Repository.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Repository.Models
{
    public class Customize
    {
        public Guid Id { get; set; }
      /*public Level? Milk { get; set; }
        public Level? Ice { get; set; }
        public Level? Sugar { get; set; }*/

        /*public Temperature? Temperature { get; set; }*/

        public string Note {  get; set; }
        public double? Extra { get; set; }

        public Guid SizeId { get; set; }
        public Size? Size { get; set; } = default!;

        public Guid ProductId { get; set; }
        public Product Product { get; set; } = default!;

        public double? Price { get; set; }

        [JsonIgnore]
        public CartItem? CartItem { get; set; }
        [JsonIgnore]
        public OrderItem? OrderItem { get; set; }
        public List<CustomizeTopping>? CustomizeToppings { get; set; } = new List<CustomizeTopping>();

    }
}
