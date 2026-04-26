namespace RSM.Domain.Models
{
    public class Customer
    {
        public int CustomerId { get; set; }
        public required string CompanyName { get; set; }
        public string? ContactName { get; set; }
        public string? City { get; set; }
    }
}