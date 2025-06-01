using System.Text.Json.Serialization;

namespace Repository.Models
{
    public class CartItem
    {
        public Guid Id { get; set; }
        public Guid CartId { get; set; }
        [JsonIgnore]
        public Cart? Cart { get; set; }

        public Guid CustomizeId { get; set; }
        public Customize? Customize { get; set; }

        public int? Quantity { get; set; }
        public double? UnitPrice { get; set; }
        public string? Description { get; set; }
    }
}