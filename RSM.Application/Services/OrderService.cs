using Microsoft.EntityFrameworkCore;
using RSM.Application.Dtos;
using RSM.Domain.Models;
using RSM.Infrastructure.Data;

namespace RSM.Application.Services
{
    public interface IOrderService
    {
        List<OrderDetailDto> GetOrders(string? name);
        List<OrderDetailDto> GetOrder(int OrderId);
    }

    public class OrderService : IOrderService
    {
        private readonly ApplicationDBContext _context;
        public OrderService(ApplicationDBContext context)
        {
            _context = context;
        }

        public List<OrderDetailDto> GetOrders(string? name)
        {
            IQueryable<Order> query = _context.Orders
                        .Include(o=> o.Customer)
                        .Include(o=> o.Shipper);

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(o => o.Customer.CompanyName.Contains(name));
            }
            var result = query.Select(o => new OrderDetailDto
                        {
                            OrderId = o.OrderId,
                            CustomerId = o.CustomerId,
                            CompanyName = o.Customer.CompanyName,
                            EmployeeId = o.EmployeeId,
                            Freight = o.Freight,
                            OrderDate = o.OrderDate,
                            RequiredDate = o.RequiredDate,
                            ShipAddress = o.ShipAddress,
                            ShipCity = o.ShipCity,
                            ShipCountry = o.ShipCountry,
                            ShipName = o.ShipName,
                            ShipPostalCode = o.ShipPostalCode,
                            ShipRegion = o.ShipRegion,
                            ShipperName = o.Shipper.CompanyName
                        });

            

            return result.ToList();
        }
        public List<OrderDetailDto> GetOrder(int OrderId)
        {
            var result = _context.Orders
                        .Include(o => o.Customer)
                        .Include(o => o.Shipper)
                        .Where(o => o.OrderId == OrderId)
                        .Select(o => new OrderDetailDto
                        {
                            OrderId = o.OrderId,
                            CustomerId = o.CustomerId,
                            CompanyName = o.Customer.CompanyName,
                            EmployeeId = o.EmployeeId,
                            Freight = o.Freight,
                            OrderDate = o.OrderDate,
                            RequiredDate = o.RequiredDate,
                            ShipAddress = o.ShipAddress,
                            ShipCity = o.ShipCity,
                            ShipCountry = o.ShipCountry,
                            ShipName = o.ShipName,
                            ShipPostalCode = o.ShipPostalCode,
                            ShipRegion = o.ShipRegion,
                            ShipperName = o.Shipper.CompanyName
                        });

            return result.ToList();
        }
    }
}