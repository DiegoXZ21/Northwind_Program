using System.Globalization;
using Microsoft.EntityFrameworkCore;
using RSM.Application.Dtos;
using RSM.Domain.Models;
using RSM.Infrastructure.Data;
using RSM.Application.Services.External;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json;

namespace RSM.Application.Services
{
    public interface IOrderService
    {

        List<OrderListDto> GetOrders(int? year, int? month, int? week, string? country);
        OrderDetailDto GetOrderDetail(int orderId);
        List<int> GetOrderYears();
        List<OrdersByCountryDto> GetOrderYearsByCountryAndYear(int year);
        List<OrdersByMonthDto> GetOrdersByMonth(int year);
        Task UpdateOrderAsync(UpdateOrderDto dto);
        Task<bool> DeleteOrderAsync(int orderId);

        Task UpdateOrderStatusAsync(int orderId, int newStatus);
    }

    public class OrderService : IOrderService
    {
        private readonly ApplicationDBContext _context;
        private readonly IGoogleAddressService _googleService;

        public OrderService(ApplicationDBContext context,IGoogleAddressService googleService)
        {
            _context = context;
            _googleService = googleService;
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

        public async Task UpdateOrderAsync(UpdateOrderDto dto)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.OrderId == dto.OrderId);

            if (order == null)
                throw new Exception("Order not found");

            if (order.Status > 1)
                throw new Exception("Order cannot be edited");

            if (dto.ShipperId <= 0)
                throw new Exception("Shipper is required");

            order.ShipperId = dto.ShipperId;

            var addressResult = await _googleService.ValidateAddressAsync(new AddressDto
            {
                Address = dto.ShippingAddress,
                City = dto.City,
                Region = dto.Region,
                Country = dto.Country,
                PostalCode = dto.PostalCode
                
            });

            if (!addressResult.IsValid)
                throw new Exception("Invalid shipping address");

            order.ShipAddress = dto.ShippingAddress;
            order.ShipCity = dto.City;
            order.ShipRegion = dto.Region;
            order.ShipCountry = dto.Country;
            order.ShipPostalCode = dto.PostalCode;

            order.Latitude = (decimal?)addressResult.Latitude;
            order.Longitude = (decimal?)addressResult.Longitude;

            if (dto.Products == null)
                dto.Products = new List<UpdateOrderDetailDto>();

            foreach (var updatedItem in dto.Products)
            {
                var existingDetail = order.OrderDetails
                    .FirstOrDefault(d => d.ProductId == updatedItem.ProductId);

                var product = await _context.Products
                    .FirstOrDefaultAsync(p => p.ProductId == updatedItem.ProductId);

                if (product == null)
                    throw new Exception($"Product {updatedItem.ProductId} not found");

                if (product.Discontinued == true)
                {
                    if (existingDetail != null)
                        continue;

                    throw new Exception($"Product {product.ProductName} is discontinued");
                }

                if (existingDetail != null)
                {
                    int diff = updatedItem.Quantity - existingDetail.Quantity;

                    if (diff > 0 && (product.UnitsInStock ?? 0) < diff)
                        throw new Exception($"Not enough stock for {product.ProductName}");

                    product.UnitsInStock = (short?)(product.UnitsInStock - diff);

                    existingDetail.Quantity = (short)updatedItem.Quantity;
                    existingDetail.Discount = (float)updatedItem.Discount;
                }
                else
                {
                    if ((product.UnitsInStock ?? 0) < updatedItem.Quantity)
                        throw new Exception($"Not enough stock for {product.ProductName}");

                    order.OrderDetails.Add(new OrderDetails
                    {
                        ProductId = updatedItem.ProductId,
                        Quantity = (short)updatedItem.Quantity,
                        Discount = (float)updatedItem.Discount,
                        UnitPrice = product.UnitPrice ?? 0
                    });

                    product.UnitsInStock = (short?)(product.UnitsInStock - updatedItem.Quantity);
                }
            }

            var toRemove = order.OrderDetails
                .Where(d => !dto.Products.Any(p => p.ProductId == d.ProductId))
                .ToList();

            foreach (var item in toRemove)
            {
                var product = await _context.Products
                    .FirstOrDefaultAsync(p => p.ProductId == item.ProductId);

                if (product != null)
                    product.UnitsInStock = (short?)(product.UnitsInStock + item.Quantity);

                _context.OrderDetails.Remove(item);
            }

            order.Status = dto.Status;

            await _context.SaveChangesAsync();
        }
    
        public async Task<bool> DeleteOrderAsync(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
                throw new Exception("Order not found");

            if (order.Status != 0)
                throw new Exception("Only pending orders can be deleted");

            if (order.OrderDetails != null && order.OrderDetails.Any())
            {
                _context.OrderDetails.RemoveRange(order.OrderDetails);
            }

            _context.Orders.Remove(order);

            await _context.SaveChangesAsync();

            return true;
        }
    
        public async Task UpdateOrderStatusAsync(int orderId, int newStatus)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
                throw new Exception("Order not found");

            var allowedTransitions = new Dictionary<int, List<int>>
            {
                { 0, new List<int> { 1, 4 } }, 
                { 1, new List<int> { 2, 4 } }, 
                { 2, new List<int> { 3 } },    
                { 3, new List<int>() },        
                { 4, new List<int>() }         
            };

            if (!allowedTransitions[order.Status].Contains(newStatus))
                throw new Exception("Invalid status transition");

            order.Status = newStatus;

            await _context.SaveChangesAsync();
        }
    }
}