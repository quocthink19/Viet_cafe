using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Models.DTOs.Response
{
    public class Response
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }

        public Response(int code, string message, object data)
        {
            Code = code;
            Message = message;
            Data = data;
        }
    }
}
