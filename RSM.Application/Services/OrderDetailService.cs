using Microsoft.EntityFrameworkCore;
using RSM.Application.Dtos;
using RSM.Domain.Models;
using RSM.Infrastructure.Data;

namespace RSM.Application.Services
{
    public interface IOrderDetailService
    {
        OrderWithProductsDto GetOrderDetail(int id);
    }

    public class OrderDetailService : IOrderDetailService
    {
        private readonly ApplicationDBContext _context;

        public OrderDetailService(ApplicationDBContext context)
        {
            _context = context;
        }

        public OrderWithProductsDto GetOrderDetail(int id)
        {
            var order = _context.Orders
                        .Include(o => o.Customer)
                        .Include(o => o.OrderDetails)
                            .ThenInclude(od => od.Product)
                        .FirstOrDefault(o => o.OrderId == id);

            if(order == null)
            {
                throw new Exception("Order not found");
            }

            return new OrderWithProductsDto
            {
                OrderId = order.OrderId,
                CustomerName = order.Customer.CompanyName,
                OrderDate = order.OrderDate ?? DateTime.MinValue,
                ShippingAddress = order.ShipAddress ?? "",
                City = order.ShipCity ?? "",
                Region = order.ShipRegion ?? "",
                Country = order.ShipCountry ?? "",
                PostalCode = order.ShipPostalCode ?? "",
                ShipperId = order.ShipperId,

                Status = order.Status,
                Freight = order.Freight ?? 0,
                Latitude = order.Latitude,
                Longitude = order.Longitude,
                Products = order.OrderDetails.Select(d => new OrderProductDto
                {
                    ProductId = d.ProductId,
                    ProductName = d.Product.ProductName,
                    UnitPrice = d.UnitPrice,
                    Quantity = d.Quantity,
                    Discount = d.Discount,
                    QuantityPerUnit = d.Product.QuantityPerUnit ?? "",
                    Discontinued = d.Product.Discontinued,
                    UnitsInStock = d.Product.UnitsInStock ?? 0
                    
                }).ToList()
            };
        }
            
    }
}