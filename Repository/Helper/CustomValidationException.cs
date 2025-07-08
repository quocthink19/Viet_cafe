using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Helper
{
    public class CustomValidationException : Exception
    {
        public object Errors { get; }
        public CustomValidationException(object errors)
            : base("Validation failed")
        {
            Errors = errors;
        }
    }
}