namespace RSM.Application.Dtos
{
    public class AvailableProductDto
    {
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public int UnitsInStock { get; set; }
        public string? QuantityPerUnit { get; set; }
    }
}