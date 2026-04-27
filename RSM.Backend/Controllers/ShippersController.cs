using Microsoft.AspNetCore.Mvc;
using RSM.Application.Services;

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
    [HttpGet("sample")]
    public IActionResult GetSample()
    {
        try
        {
            var shippers = _shipperService.GetShippers();
            return Ok(shippers);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al consultar Shippers: {ex.Message}");
        }
    }

}