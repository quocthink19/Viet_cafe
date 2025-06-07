namespace Repository.Models.DTOs.Response
{
    public class CartItemResponse
    {
        public Guid Id { get; set; }
        public int? Quantity { get; set; }
        public double? UnitPrice { get; set; }
      /*  public string? Name { get; set; }
        public string? Size { get; set; }
        public string? Note { get; set; }
        public string? Description { get; set; }*/
        public CustomizeResponse? Customize { get; set; }

    }
}