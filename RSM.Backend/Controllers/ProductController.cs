using Microsoft.AspNetCore.Mvc;
using RSM.Application.Services;

namespace RSM.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        // GET: api/Product
        [HttpGet("sample")]
        public IActionResult GetProducts(string? name, string? category)
        {
            try
            {
                var products = _productService.GetProducts(name, category);
                return Ok(products);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error when trying to get products: {ex.Message}");
            }
        }
    }
}