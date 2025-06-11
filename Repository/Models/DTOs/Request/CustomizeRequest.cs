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
      

        public string Note {  get; set; }
        public Guid SizeId { get; set; }
        public Guid ProductId { get; set; }
        public int Quanity { get; set; }
        public List<CustomizeToppingDto> CustomizeToppings { get; set; } = new();

    }
}
