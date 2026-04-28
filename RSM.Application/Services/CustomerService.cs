using RSM.Domain.Models;
using RSM.Infrastructure.Data;

namespace RSM.Application.Services
{
    public interface ICustomerService
    {
        List<Customer> GetCustomers(string? name, string? city, string? country);
    }

    public class CustomerService : ICustomerService
    {
        private readonly ApplicationDBContext _context;
        public CustomerService(ApplicationDBContext context)
        {
            _context = context;
        }

        public List<Customer> GetCustomers(string? name, string? city, string? country)
        {
            var query = _context.Customers.AsQueryable();
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(c => c.CompanyName.Contains(name));
            }

            if (!string.IsNullOrEmpty(city))
            {
                query = query.Where(c => c.City.Contains(city));
            }

            if (!string.IsNullOrEmpty(country))
            {
                query = query.Where(c => c.Country.Contains(country));
            }
            
            return query.ToList();
        }
    }
}