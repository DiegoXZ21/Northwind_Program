using Microsoft.AspNetCore.Mvc;
using RSM.Application.Services;
using RSM.Application.Dtos;

namespace RSM.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _orderService;

        public ReportController(IReportService orderService)
        {
            _orderService = orderService;
        }

        

        [HttpPost("orders/export")]
        public IActionResult ExportOrders([FromBody] ExportOrdersRequestDto request)
        {
            var data = _orderService.GetOrdersForExport(request);

            return Ok( new
            {
                message = "Export data ready",
                count = data.Count(),
                data
            });
        }
    }
}