using System.Text.Json.Serialization;

namespace Repository.Models
{
    public class CustomizeTopping
    {
        public Guid CustomizeId { get; set; }
        [JsonIgnore]
        public Customize Customize { get; set; } = default!;

        public Guid ToppingId { get; set; }
        public Topping Topping { get; set; } = default!;

        public int Quantity {  get; set; }
    }
}