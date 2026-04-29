namespace RSM.Application.Dtos
{
    public class CustomerDto
    {
        public string CustomerId { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string? City { get; set; }
        public string? Country { get; set; }
    }
}