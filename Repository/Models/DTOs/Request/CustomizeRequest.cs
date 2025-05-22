using Repository.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Models.DTOs.Request
{
    public class CustomizeRequest
    {
        public Level? Milk { get; set; }
        public Level? Ice { get; set; }
        public Level? Sugar { get; set; }
        public Temperature? Temperature { get; set; }

        public Guid SizeId { get; set; }
        public Guid ProductId { get; set; }
        public List<CustomizeToppingDto> CustomizeToppings { get; set; } = new();

    }
}
