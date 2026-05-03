namespace RSM.Application.Dtos
{
    public class ProductInventoryDto
    {
        public required int ProductId { get; set; }
        public required string ProductName { get; set; }
        public short? UnitsInStock { get; set; }
        public required string CategoryName { get; set; }

    }
}