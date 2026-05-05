using Microsoft.EntityFrameworkCore;
using RSM.Application.Dtos;
using RSM.Application.Services;
using RSM.Infrastructure.Data;
using System.Globalization;

namespace RSM.Application.Services
{
    public interface IReportService
    {
        List<OrderListDto> GetOrdersForExport(ExportOrdersRequestDto request);
    }
    public class ReportService : IReportService
    {
        private readonly ApplicationDBContext _context;

        public ReportService(ApplicationDBContext context)
        {
            _context = context;
        }

        public List<OrderListDto> GetOrdersForExport(ExportOrdersRequestDto request)
        {
            var query = _context.Orders
                        .Include(o => o.Customer)
                        .Include(o => o.OrderDetails)
                        .AsQueryable();

            if (request.OrderIds != null && request.OrderIds.Any())
            {
                query = query.Where(o => request.OrderIds.Contains(o.OrderId));
            }

            if (request.Year.HasValue)
            {
                query = query.Where(o => o.OrderDate.HasValue && o.OrderDate.Value.Year == request.Year.Value);
            }

            if (request.Month.HasValue)
            {
                query = query.Where(o => o.OrderDate.HasValue && o.OrderDate.Value.Month == request.Month.Value);
            }

            if (!string.IsNullOrEmpty(request.Country))
            {
                query = query.Where(o => o.ShipCountry != null && o.ShipCountry.Contains(request.Country));
            }

            return query
                    .AsEnumerable()
                    .Select(o => new OrderListDto
                    {
                        OrderId = o.OrderId,
                        CustomerName = o.Customer.CompanyName,
                        OrderDate = o.OrderDate ?? DateTime.MinValue,
                        Country = o.ShipCountry ?? "",
                        Status = o.Status switch
                        {
                            0 => "Pending",
                            1 => "Processed",
                            2 => "Shipped",
                            3 => "Completed",
                            4 => "Cancelled",
                            _ => "Unknown"
                        },

                        TotalAmount = o.OrderDetails
                            .Sum(d => d.UnitPrice * d.Quantity * (1 - (decimal)d.Discount)),

                        ProductCount = o.OrderDetails.Count

                    }).ToList();
        }
    }
}