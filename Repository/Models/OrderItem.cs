using System.Text.Json.Serialization;

namespace Repository.Models
{
    public class OrderItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int? Quantity { get; set; }
        public double? UnitPrice { get; set; }
        public long OrderId { get; set; }
        [JsonIgnore]
        public Order Order { get; set; }     
    }
}