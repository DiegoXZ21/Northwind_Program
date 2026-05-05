namespace RSM.Application.Dtos
{
    public class OrderItemDto
    {
        public string ProductName { get; set; } = string.Empty;
        public short Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public float Discount { get; set; }

        public decimal Subtotal => Quantity * UnitPrice;
        public decimal DiscountAmount => Subtotal * (decimal)Discount;
        public decimal Total => Subtotal - DiscountAmount;
    }
}