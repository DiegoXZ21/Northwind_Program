namespace RSM.Domain.Models
{
    public class OrderDetails
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
        public decimal UnitPrice { get; set; }
        public short Quantity { get; set; }
        public float Discount { get; set; }

        public Order Order { get; set; } = null!;
    }
}