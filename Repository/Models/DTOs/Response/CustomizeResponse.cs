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
        public Guid Id { get; set; }
        
        public string Note {  get; set; }
        public string Size { get; set; }
        public string Product { get; set; }
        public double? Price { get; set; }
        public double? Extra { get; set; }

        public List<CustomizeToppingResponse> CustomizeToppings { get; set; } = new();
    }
}
