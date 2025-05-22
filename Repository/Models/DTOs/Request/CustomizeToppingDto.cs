namespace Repository.Models.DTOs.Request
{
    public class CustomizeToppingDto
    {
        public Guid ToppingId { get; set; }
        public int? Quantity { get; set; }
    }
}