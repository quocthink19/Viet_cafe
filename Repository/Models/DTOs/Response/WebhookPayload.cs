using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Models.DTOs.Response
{
    public class WebhookPayload
    {
        public long orderCode { get; set; }
        public string status { get; set; }
    }
}
