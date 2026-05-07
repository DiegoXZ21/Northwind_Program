using Xunit;
using Microsoft.EntityFrameworkCore;
using Moq;
using RSM.Application.Services;
using RSM.Application.Dtos;
using RSM.Infrastructure.Data;
using RSM.Domain.Models;
using RSM.Application.Services.External;

public class EmployeeServiceTest
{
    private ApplicationDBContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDBContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDBContext(options);
    }

    [Fact]
    public void GetEmployees_ShouldReturnMappedEmployees()
    {
        var context = GetDbContext();

        context.Employees.Add(new Employee
        {
            EmployeeId = 1,
            FirstName = "Diego",
            LastName = "Guerrero"
        });

        context.SaveChanges();

        var service = new EmployeeService(context);

        var result = service.GetEmployees();

        Assert.Single(result);
        Assert.Equal(1, result[0].EmployeeId);
        Assert.Equal("Diego Guerrero", result[0].EmployeeName);
    }

    [Fact]
    public void GetEmployees_ShouldReturnEmptyList_WhenNoEmployees()
    {
        var context = GetDbContext();

        var service = new EmployeeService(context);

        var result = service.GetEmployees();

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void GetEmployees_ShouldConcatenateFirstAndLastName()
    {
        var context = GetDbContext();

        context.Employees.Add(new Employee
        {
            EmployeeId = 1,
            FirstName = "Ana",
            LastName = "Lopez"
        });

        context.SaveChanges();

        var service = new EmployeeService(context);

        var result = service.GetEmployees();

        Assert.Equal("Ana Lopez", result.First().EmployeeName);
    }
}