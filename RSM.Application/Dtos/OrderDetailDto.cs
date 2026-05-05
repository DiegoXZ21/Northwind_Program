namespace RSM.Application.Dtos
{
    public class OrderDetailDto
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal Freight { get; set; }

        public string CustomerName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;

        public List<OrderItemDto> Items { get; set; } = new();

        public decimal Total { get; set; }
    }

}