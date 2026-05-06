namespace RSM.Application.Dtos
{
    public class UpdateOrderDetailDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Discount { get; set; }
    }
}