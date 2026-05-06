using Microsoft.AspNetCore.Mvc;
using RSM.Application.Services;

namespace RSM.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderDetailController : ControllerBase
    {
        public readonly IOrderDetailService _orderDetailService;

        public OrderDetailController(IOrderDetailService orderDetailService)
        {
            _orderDetailService = orderDetailService;
        }

        [HttpGet("{id}")]
        public IActionResult GetOrderDetail(int id)
        {
            try
            {
                var data = _orderDetailService.GetOrderDetail(id);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error when trying to get order detail info: {ex.Message}");
            }
        }
    }
}