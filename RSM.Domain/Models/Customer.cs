namespace RSM.Domain.Models
{
    public class Customer
    {
        public required string CustomerId { get; set; }
        public required string CompanyName { get; set; }
        public string? ContactName { get; set; }
        public string? Address { get; set; }
        public string? Region { get; set; }
        public string? City { get; set; }
        public string? PostalCode { get; set; }
        public string? Country { get; set; }
        public string? Phone { get; set; }

        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}