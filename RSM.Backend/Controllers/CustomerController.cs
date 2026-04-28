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

        // GET: api/Customer ?name=abc&city=xyz&country=def
        [HttpGet("sample")]
        public IActionResult GetCustomers([FromQuery] string? name, [FromQuery] string? city, [FromQuery] string? country)
        {
            try
            {
                var customers = _customerService.GetCustomers(name, city, country);
                return Ok(customers);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error when trying to get customers: {ex.Message}");
            }
        }
    }
}