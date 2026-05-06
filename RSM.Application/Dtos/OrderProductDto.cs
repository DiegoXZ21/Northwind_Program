namespace RSM.Application.Dtos
{
    public class OrderProductDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public double Discount { get; set; }
        public string QuantityPerUnit { get; set; } = string.Empty;
        public bool Discontinued { get; set; }
        public short UnitsInStock {get; set; }
    }
}