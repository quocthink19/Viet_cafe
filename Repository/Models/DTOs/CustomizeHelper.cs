using Repository.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Models.DTOs
{
    public class CustomizeHelper
    {
        public static string BuildDescription(Customize customize)
        {
            if (customize == null) return string.Empty;

            var productName = customize.Product?.Name ?? "Tên SP";
            var size = customize.Size?.Name ?? "Size?";
            var ice = FormatLevel(customize.Ice, "đá");
            var milk = FormatLevel(customize.Milk, "sữa");
            var sugar = FormatLevel(customize.Sugar, "ngọt");

            return $"{productName} size {size} {ice} {milk} {sugar}";
        }
        private static string FormatLevel(Level? level, string label)
        {
            return level switch
            {
                Level.LESS => $"ít {label}",
                Level.NORMAL => $"vừa {label}",
                Level.MORE => $"nhiều {label}",
                _ => $"mặc định {label}"
            };
        }
    }
}

