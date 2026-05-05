using System.Globalization;
using Microsoft.EntityFrameworkCore;
using RSM.Application.Dtos;
using RSM.Domain.Models;
using RSM.Infrastructure.Data;

namespace RSM.Application.Services
{
    public interface IOrderService
    {

        List<OrderListDto> GetOrders(int? year, int? month, int? week, string? country);
        OrderDetailDto GetOrderDetail(int orderId);
        List<int> GetOrderYears();
        List<OrdersByCountryDto> GetOrderYearsByCountryAndYear(int year);
        List<OrdersByMonthDto> GetOrdersByMonth(int year);
    }

    public class OrderService : IOrderService
    {
        private readonly ApplicationDBContext _context;
        public OrderService(ApplicationDBContext context)
        {
            _context = context;
        }

        public List<OrderListDto> GetOrders(int? year, int? month, int? week, string? country)
        {
            var query = _context.Orders
                        .Include(o => o.Customer)
                        .Include(o => o.OrderDetails)
                        .AsQueryable();

            if (year.HasValue)
            {
                query = query.Where(o => 
                    o.OrderDate.HasValue &&
                    o.OrderDate.Value.Year == year.Value
                );
            }

            if (week.HasValue)
            {
                query = query.AsEnumerable()
                    .Where(o =>
                        o.OrderDate.HasValue &&
                        CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(
                            o.OrderDate.Value,
                            CalendarWeekRule.FirstDay,
                            DayOfWeek.Monday
                        ) == week.Value
                    )
                    .AsQueryable();
            }
            
            if (month.HasValue)
            {
                query = query.Where(o => 
                    o.OrderDate.HasValue &&
                    o.OrderDate.Value.Month == month.Value
                );
            }

            if (!string.IsNullOrEmpty(country))
            {
                query = query.Where(o =>
                    o.ShipCountry != null &&
                    o.ShipCountry.Contains(country)
                );
            }

            var data = query.Select(o => new
            {
                o.OrderId,
                CustomerName = o.Customer.CompanyName,
                OrderDate = o.OrderDate ?? DateTime.MinValue,
                Country = o.ShipCountry ?? "",
                o.Status,

                TotalAmount = o.OrderDetails
                    .Sum(d => d.UnitPrice * d.Quantity * (1 - (decimal)d.Discount)),

                ProductCount = o.OrderDetails.Count
            }).ToList();

            return data.Select(o => new OrderListDto
            {
                OrderId = o.OrderId,
                CustomerName = o.CustomerName,
                OrderDate = o.OrderDate,
                Country = o.Country,

                Status = o.Status switch
                {
                    0 => "Pending",
                    1 => "Processing",
                    2 => "Shipped",
                    3 => "Completed",
                    4 => "Cancelled",
                    _ => "Unknown"
                },

                TotalAmount = o.TotalAmount,
                ProductCount = o.ProductCount
            }).ToList();
        }
        
        public OrderDetailDto GetOrderDetail(int orderId)
        {
            var order = _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails)
                .ThenInclude(d => d.Product)
                .FirstOrDefault(o => o.OrderId == orderId);

            if (order == null)
                throw new Exception("Order not found");

            var items = order.OrderDetails.Select(d => new OrderItemDto
            {
                ProductName = d.Product.ProductName,
                Quantity = d.Quantity,
                UnitPrice = d.UnitPrice,
                Discount = d.Discount
            }).ToList();

            return new OrderDetailDto
            {
                OrderId = order.OrderId,
                OrderDate = order.OrderDate ?? DateTime.Now,
                Status = order.Status switch
                {
                    0 => "Pending",
                    1 => "Processed",
                    2 => "Shipped",
                    3 => "Completed",
                    4 => "Cancelled",
                    _ => "Unknown"
                },
                Freight = order.Freight ?? 0,

                CustomerName = order.Customer.CompanyName,
                Address = order.ShipAddress ?? "",
                City = order.ShipCity ?? "",
                Country = order.ShipCountry ?? "",

                Items = items,

                Total = items.Sum(i => i.Total) + (order.Freight ?? 0)
            };
        }

        public List<int> GetOrderYears()
        {
            return _context.Orders
                .Where(o => o.OrderDate != null)
                .Select(o => o.OrderDate!.Value.Year)
                .Distinct()
                .OrderBy(y => y)
                .ToList();
        }

        public List<OrdersByCountryDto> GetOrderYearsByCountryAndYear(int year)
        {
            return _context.Orders
                .Where(o => o.OrderDate != null && o.OrderDate.Value.Year == year)
                .GroupBy(o => o.ShipCountry ?? "Unknown")
                .Select(g => new OrdersByCountryDto
                {
                    Country = g.Key,
                    Total = g.Count()
                })
                .OrderByDescending(x => x.Total)
                .ToList();
        }

        public List<OrdersByMonthDto> GetOrdersByMonth(int year)
        {
            return _context.Orders
            .Where(o => o.OrderDate.HasValue && o.OrderDate.Value.Year == year)
            .GroupBy(o => o.OrderDate!.Value.Month)
            .Select(g => new OrdersByMonthDto
            {
                Month = g.Key,
                Total = g.Count()
            })
            .OrderBy(x => x.Month)
            .ToList();
        }
    }
}