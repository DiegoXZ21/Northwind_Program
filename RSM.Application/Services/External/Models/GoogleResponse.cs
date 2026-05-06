namespace RSM.Application.Services.External.Models
{
    public class GoogleResponse
    {
        public Result? result { get; set; }

        public class Result
        {
            public Geocode? geocode { get; set; }
            public Address? address { get; set; }
            public Verdict? verdict { get; set; }
        }

        public class Verdict
        {
            public string? possibleNextAction { get; set; }
        }

        public class Geocode
        {
            public Location? location { get; set; }
        }

        public class Location
        {
            public double latitude { get; set; }
            public double longitude { get; set; }
        }

        public class Address
        {
            public string? formattedAddress { get; set; }
        }
    }
}