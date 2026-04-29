using RSM.Application.Dtos;
using RSM.Domain.Models;
using RSM.Infrastructure.Data;

namespace RSM.Application.Services
{
    public interface IEmployeeService
    {
        List<EmployeeDto> GetEmployees();
    }

    public class EmployeeService : IEmployeeService
    {
        private readonly ApplicationDBContext _context;

        public EmployeeService(ApplicationDBContext context)
        {
            _context = context;
        }

        public List<EmployeeDto> GetEmployees()
        {
            return _context.Employees.Select(e => new EmployeeDto
            {
                EmployeeId = e.EmployeeId,
                EmployeeName = $"{e.FirstName} {e.LastName}"
            }).ToList();
        }
    }
}