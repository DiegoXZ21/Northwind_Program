using Microsoft.AspNetCore.Mvc;
using RSM.Application.Services;

namespace RSM.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        public readonly IOrderService _orderService;
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // GET: api/Order ?name=abc
        [HttpGet("sample")]
        public IActionResult GetOrders([FromQuery] string? name)
        {
            try
            {
                var orders = _orderService.GetOrders(name);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error when trying to get orders: {ex.Message}");
            }
        }
    }
}