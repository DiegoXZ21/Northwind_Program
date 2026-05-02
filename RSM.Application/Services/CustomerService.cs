using RSM.Application.Dtos;
using RSM.Domain.Models;
using RSM.Infrastructure.Data;

namespace RSM.Application.Services
{
    public interface ICustomerService
    {
        List<CustomerDto> GetCustomerDtos(string? name, string? city, string? country);
        List<CustomerShippingDto> GetCustomerShippingDtos(string customerId);
    }

    public class CustomerService : ICustomerService
    {
        private readonly ApplicationDBContext _context;
        public CustomerService(ApplicationDBContext context)
        {
            _context = context;
        }
        public List<CustomerDto> GetCustomerDtos(string? name, string? city, string? country)
        {
            var query = _context.Customers.AsQueryable();
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(c => (c.CompanyName ?? "").Contains(name));
            }
            if (!string.IsNullOrEmpty(city))
            {
                query = query.Where(c => (c.City ?? "").Contains(city));
            }
            if (!string.IsNullOrEmpty(country))
            {
                query = query.Where(c => (c.Country ?? "").Contains(country));
            }
            return query.Select(c => new CustomerDto
            {
                CustomerId = c.CustomerId,
                CompanyName = c.CompanyName,
                City = c.City,
                Country = c.Country
            }).ToList();
        }
        public List<CustomerShippingDto> GetCustomerShippingDtos(string customerId)
        {
            var query = _context.Customers.AsQueryable();
            if (!string.IsNullOrEmpty(customerId))
            {
                query = query.Where(c => c.CustomerId == customerId);
            }
            return query.Select(c => new CustomerShippingDto
            {
                Address = c.Address ?? "",
                City = c.City ?? "",
                Region = c.Region ?? "",
                PostalCode = c.PostalCode ?? "",
                Country = c.Country ?? ""
            }).ToList();
        }
    }
}