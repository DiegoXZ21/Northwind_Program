using Microsoft.AspNetCore.Mvc;
using RSM.Application.Services;

namespace RSM.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        // GET: api/Customer/dto ?name=abc&city=xyz&country=def
        [HttpGet("dto")]
        public IActionResult GetCustomerDtos([FromQuery] string? name, [FromQuery] string? city, [FromQuery] string? country)
        {
            try
            {
                var customerDtos = _customerService.GetCustomerDtos(name, city, country);
                return Ok(customerDtos);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error when trying to get customer: {ex.Message}");
            }
        }

        // GET: api/Customer/dto/{id}
        [HttpGet("dto/{id}")]
        public IActionResult GetCustomerShippingDtos([FromRoute] string id)
        {
            try
            {
                var customerShippingDtos = _customerService.GetCustomerShippingDtos(id);
                return Ok(customerShippingDtos);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error when trying to get customer shipping info: {ex.Message}");
            }
        }
    }
}