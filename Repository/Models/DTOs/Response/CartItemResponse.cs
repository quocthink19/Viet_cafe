namespace Repository.Models.DTOs.Response
{
    public class CartItemResponse
    {
        public Guid Id { get; set; }
        public int? Quantity { get; set; }
        public double? UnitPrice { get; set; }
        public CustomizeResponse? Customize { get; set; }
    }
}