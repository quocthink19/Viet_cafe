using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Models.DTOs.Response
{
    public class VnPayResponseDTO
    {
        public bool IsSuccess { get; set; }
        public String OrderId { get; set; }
        public string VnPayResponseCode { get; set; }
        public double Amount { get; set; }
    }
}
