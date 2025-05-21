using Repository.Models.DTOs.Request;
using Repository.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Models.DTOs.Response
{
    public class CustomizeResponse
    {
        public Level Milk { get; set; }
        public Level Ice { get; set; }
        public Level Sugar { get; set; }
        public Temperature Temperature { get; set; }

        public string Size { get; set; }
        public string Product { get; set; }
        public double? Price { get; set; }
        public double? Extra { get; set; }

        public List<CustomizeToppingResponse> CustomizeToppings { get; set; } = new();
    }
}
