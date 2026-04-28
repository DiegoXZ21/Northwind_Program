namespace RSM.Domain.Models
{
    public class Product
    {
        public required int ProductId { get; set; }
        public required string ProductName { get; set; }
        public int? CategoryId { get; set; }
        public string? QuantityPerUnit { get; set; }
        public decimal? UnitPrice { get; set; }
        public short? UnitsInStock { get; set; }
        public bool Discontinued { get; set; }

        public Category Category {get; set;} = null!;
    }
}