using Repository.Models;
using Repository.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Helper
{
    public class CustomizeHelper
    {
        public static string BuildDescription(Customize customize)
        {
            if (customize == null) return string.Empty;

            var productName = customize.Product?.Name ?? "Tên SP";
            var size = customize.Size?.Name ?? "Size?";
            var note = customize.Note ?? "Note";

            var toppingList = customize.CustomizeToppings?
                .Select(ct => ct.Topping?.Name)
                .Where(name => !string.IsNullOrEmpty(name))
                .ToList();

            var toppings = toppingList != null && toppingList.Any()
                ? $"thêm: {string.Join(", ", toppingList)}"
                : "";

            return $"{productName} size {size} {note} ,{toppings}";
        }
        }
    }


