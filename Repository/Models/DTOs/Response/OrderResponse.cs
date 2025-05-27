using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Models.DTOs.Response
{
    public  class OrderResponse
    {
        public double? TotalAmount { get; set; }
        public double? DiscountPrice { get; set; }
        public double? FinalPrice { get; set; }

        public string QRcode;



    }
}
