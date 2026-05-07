namespace RSM.Application.Dtos
{
    public class CreateOrderDto
    {
        public string? CustomerId { get; set; }
        public int EmployeeId { get; set; }
        public int ShipperId { get; set; }

        public string? ShippingAddress { get; set; }
        public string? ShipName {get; set;}
        public decimal Freight { get; set; }
        public string? City { get; set; }
        public string? Region { get; set; }
        public string? Country { get; set; }
        public string? PostalCode { get; set; }

        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }

        public required List<UpdateOrderDetailDto> Products { get; set; }
    }
}