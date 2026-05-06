using Microsoft.AspNetCore.Mvc;
using RSM.Application.Services.External;
using RSM.Application.Dtos;
using System.Text.Json;

namespace RSM.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GoogleAddressController : ControllerBase
    {
        private readonly IGoogleAddressService _service;

        public GoogleAddressController(IGoogleAddressService service)
        {
            _service = service;
        }

        [HttpPost("validate")]
        public async Task<IActionResult> Validate(AddressDto dto)
        {
            var result = await _service.ValidateAddressAsync(dto);
            Console.WriteLine("VALIDATION RESULT:");
            Console.WriteLine(JsonSerializer.Serialize(result));
            return Ok(result);
        }
    }
}