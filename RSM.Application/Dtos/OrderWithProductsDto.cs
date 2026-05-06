namespace RSM.Application.Dtos
{
    public class OrderWithProductsDto
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public int Status { get; set; }
        public string? ShippingAddress { get; set; }
        public string? City { get; set; }
        public string? Region { get; set; }
        public string? Country { get; set; }
        public string? PostalCode { get; set; }

        public int? ShipperId { get; set; }

        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }

        public List<OrderProductDto> Products { get; set; } = new();
    }
}