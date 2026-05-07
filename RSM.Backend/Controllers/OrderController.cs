using Microsoft.AspNetCore.Mvc;
using RSM.Application.Services;
using RSM.Application.Dtos;

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
        public IActionResult GetOrders(int? year, int? month, int? week, string? country, int? status)
        {
            var result = _orderService.GetOrders(year, month, week, country, status);
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

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] UpdateOrderDto dto)
        {
            if (id != dto.OrderId)
                return BadRequest("Invalid order id");

            try
            {
                await _orderService.UpdateOrderAsync(dto);

                return Ok("Order updated succesfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error updating order: {ex.Message}");
            }
        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            try
            {
                await _orderService.DeleteOrderAsync(id);

                return Ok("Order deleted successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateOrderStatusDto dto)
        {
            try
            {
                await _orderService.UpdateOrderStatusAsync(id, dto.Status);

                return Ok(new
                {
                    message = "Status updated successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(CreateOrderDto dto)
        {
            try
            {
                await _orderService.CreateOrderAsync(dto);

                return Ok(new
                {
                    message = "Order created successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}