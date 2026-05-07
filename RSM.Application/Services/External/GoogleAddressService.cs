using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using RSM.Application.Dtos;
using RSM.Application.Services.External.Models;

namespace RSM.Application.Services.External
{
    public interface IGoogleAddressService
    {
        Task<AddressValidationResult> ValidateAddressAsync(AddressDto dto);
    }
    public class GoogleAddressService : IGoogleAddressService
    {
        private readonly HttpClient _http;
        private readonly IConfiguration _config;

        public GoogleAddressService(HttpClient http, IConfiguration config)
        {
            _http = http;
            _config = config;
        }

        public async Task<AddressValidationResult> ValidateAddressAsync(AddressDto dto)
        {
            var request = new
            {
                address = new
                {
                    addressLines = new[]
                    {
                        dto.Address,
                        dto.City,
                        dto.Region,
                        dto.PostalCode
                        
                    }
                }
            };
            var apiKey = _config["Google:ApiKey"];

            var url =
            $"https://addressvalidation.googleapis.com/v1:validateAddress?key={apiKey}";

            var response = await _http.PostAsJsonAsync(url, request);

            if (!response.IsSuccessStatusCode)
            {
                return new AddressValidationResult
                {
                    IsValid = false
                };
            }

            var data = await response.Content.ReadFromJsonAsync<GoogleResponse>();

            var location = data?.result?.geocode?.location;

            return new AddressValidationResult
            {
                Latitude = location?.latitude,
                Longitude = location?.longitude,
                FormattedAddress = data?.result?.address?.formattedAddress,
                IsValid = location != null
            };
        }
    
    }
}