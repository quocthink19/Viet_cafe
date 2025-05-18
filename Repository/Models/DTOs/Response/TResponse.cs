using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Models.DTOs.Response
{
    public class TResponse<T>
    {
        public string Message { get; set; }
        public T Data { get; set; }

        public TResponse()
        {
        }
        public TResponse(string message, T data)
        {
            Message = message;
            Data = data;
        }
    }
}
