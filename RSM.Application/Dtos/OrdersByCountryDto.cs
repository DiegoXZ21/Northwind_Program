namespace RSM.Application.Dtos
{
    public class OrdersByCountryDto
    {
        public string Country { get; set; } = string.Empty;
        public int Total { get; set; }
    }
}