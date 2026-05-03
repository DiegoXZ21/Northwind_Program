using Microsoft.AspNetCore.Mvc;
using RSM.Application.Dtos;
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

        // GET: api/Product/inventory
        [HttpGet("inventory")]
        public IActionResult GetInventory(string? name, string? category)
        {
            try
            {
                var inventory = _productService.GetInventory(name, category);
                return Ok(inventory);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error when trying to get inventory: {ex.Message}");
            }
        }

        // PATCH: api/Product/inventory/{id}
        [HttpPatch("inventory/{id}")]
        public IActionResult UpdateInventory(int id, [FromBody] UpdateInventoryDto dto)
        {
            try
            {
                _productService.UpdateInventory(id, dto.UnitsInStock);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error when trying to update inventory: {ex.Message}");
            }
        }
    }
}