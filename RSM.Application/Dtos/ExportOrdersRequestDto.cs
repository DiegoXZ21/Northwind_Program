namespace RSM.Application.Dtos
{
    public class ExportOrdersRequestDto
    {
        public List<int> OrderIds { get; set; } = new();
        
        public int? Year { get; set; }
        public int? Month { get; set; }
        public string? Country { get; set; }
    }
}