using RSM.Domain.Models;
using RSM.Infrastructure.Data;

namespace RSM.Application.Services
{
    public interface ICustomerService
    {
        List<Customer> GetCustomers();
    }

    public class CustomerService : ICustomerService
    {
        private readonly ApplicationDBContext _context;
        public CustomerService(ApplicationDBContext context)
        {
            _context = context;
        }

        public List<Customer> GetCustomers()
        {
            return _context.Customers.ToList();
        }
    }
}