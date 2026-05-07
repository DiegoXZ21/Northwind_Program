using Microsoft.AspNetCore.Mvc;
using RSM.Application.Services;

namespace RSM.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShippersController : ControllerBase
    {
        private readonly IShipperService _shipperService;
        public ShippersController(IShipperService shipperService)
        {
            _shipperService = shipperService;
        }

        // GET: api/shippers
                [HttpGet("shippers")]
        public IActionResult GetShippers()
        {
            try
            {
                var result = _shipperService.GetShippers();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error getting shippers: {ex.Message}");
            }
        }

    }
}