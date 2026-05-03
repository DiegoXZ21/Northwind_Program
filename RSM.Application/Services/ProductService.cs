using RSM.Application.Dtos;
using RSM.Domain.Models;
using RSM.Infrastructure.Data;

namespace RSM.Application.Services
{
    public interface IProductService
    {
        List<Product> GetProducts(string? name, string? category);
        List<ProductInventoryDto> GetInventory(string? name, string? category);

        void UpdateInventory(int productId, short unitsInStock);
    }

    public class ProductService : IProductService
    {
        private readonly ApplicationDBContext _context;
        public ProductService(ApplicationDBContext context)
        {
            _context = context;
        }

        public List<Product> GetProducts(string? name, string? category)
        {
            var query = _context.Products.AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(p => p.ProductName.Contains(name));
            }
            
            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(p => p.Category.CategoryName == category);
            }

            return query.ToList();
        }

        public List<ProductInventoryDto> GetInventory(string? name, string? category)
        {
            var query = _context.Products.AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(p => p.ProductName.Contains(name));
            }
            
            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(p => p.Category.CategoryName.Contains(category));
            }

            return query.Select(p => new ProductInventoryDto
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                UnitsInStock = p.UnitsInStock,
                CategoryName = p.Category.CategoryName
            }).ToList();
        }

        public void UpdateInventory(int productId, short unitsInStock)
        {
            var product = _context.Products.Find(productId);
            if (product == null)
            {
                throw new Exception("Product not found");
            }

            product.UnitsInStock = unitsInStock;
            _context.SaveChanges();
        }


    }
}