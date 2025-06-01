using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Helper
{
    public class VnPayConfiguration
    {
        public string ReturnUrl { get; set; } = null!;
        public string PaymentUrl { get; set; } = null!;
        public string TmnCode { get; set; } = null!;
        public string HashSecret { get; set; } = null!;
        public string Version { get; set; } = null!;
        public string Command { get; set; } = null!;
        public string CurrCode { get; set; } = null!;
        public string Locale { get; set; } = null!;
    }
}
