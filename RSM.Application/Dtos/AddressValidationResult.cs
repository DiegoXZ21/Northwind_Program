namespace RSM.Application.Dtos
{
    public class AddressValidationResult
    {
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? FormattedAddress { get; set; }
        public bool IsValid { get; set; }
    }
}