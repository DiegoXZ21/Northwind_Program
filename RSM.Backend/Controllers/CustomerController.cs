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

        // GET: api/Customer
        [HttpGet("sample")]
        public IActionResult GetSample()
        {
            try
            {
                var customers = _customerService.GetCustomers();
                return Ok(customers);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error when trying to get customers: {ex.Message}");
            }
        }
    }
}