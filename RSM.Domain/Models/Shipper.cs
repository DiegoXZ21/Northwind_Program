namespace RSM.Domain.Models
{
    public class Shipper
    {
        public int ShipperId { get; set; }
        public required string CompanyName { get; set; }
        public string? Phone { get; set; }
    }
}