using Repository.Models.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        [Range(1, int.MaxValue, ErrorMessage = "số lượng nên lớn hơn 0")]
        public int Quanity { get; set; }
        public List<CustomizeToppingDto> CustomizeToppings { get; set; } = new();

    }
}
