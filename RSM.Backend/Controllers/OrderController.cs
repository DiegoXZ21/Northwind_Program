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

        [HttpGet]
        public IActionResult GetOrders(int? year, int? month, int? week, string? country)
        {
            var result = _orderService.GetOrders(year, month, week, country);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public IActionResult GetOrderDetail(int id)
        {
            var result = _orderService.GetOrderDetail(id);
            return Ok(result);
        }

        [HttpGet("years")]
        public IActionResult GetYears()
        {
            try
            {
                var years = _orderService.GetOrderYears();
                return Ok(years);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error when trying to get order years: {ex.Message}");
            }
        }

        [HttpGet("countries-by-year")]
        public IActionResult GetCountriesByYear([FromQuery] int year)
        {
            try
            {
                var data = _orderService.GetOrderYearsByCountryAndYear(year);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error when trying to get countries by year: {ex.Message}");
            }
            
        }

        [HttpGet("monthly-orders")]
        public IActionResult GetMonthlyOrders(int year)
        {
            try
            {
                var result = _orderService.GetOrdersByMonth(year);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error getting monthly orders: {ex.Message}");
            }
        }
    }
}