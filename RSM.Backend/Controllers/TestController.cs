using Microsoft.AspNetCore.Mvc;
using RSM.Infrastructure.Data;

namespace RSM.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public TestController(ApplicationDBContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult Get() => Ok("API is working well.");

        //Verifying is the app is consulting the database correctly
        [HttpGet("orders-count")]
        public IActionResult OrdersCount()
        {
            try
            {
                var count = _context.Orders.Count();
                return Ok($"Conexión exitosa. Total de órdenes: {count}");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error de conexión: {ex.Message}");
            }
        }

        [HttpGet("orders-sample")]
        public IActionResult OrdersSample()
        {
            try
            {
                var orders = _context.Orders
                                    .Take(10)
                                    .Select(o => new {
                                        o.OrderId,
                                        o.CustomerId,
                                        o.OrderDate,
                                        o.ShipCity}).
                                    ToList();
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error de conexión: {ex.Message}");
            }
        }
    }
}