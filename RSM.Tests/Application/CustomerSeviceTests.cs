using Xunit;
using Microsoft.EntityFrameworkCore;
using Moq;
using RSM.Application.Services;
using RSM.Application.Dtos;
using RSM.Infrastructure.Data;
using RSM.Domain.Models;
using RSM.Application.Services.External;

public class CustomerServiceTests
{
    private ApplicationDBContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDBContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDBContext(options);
    }

    [Fact]
    public void GetCustomerDtos_ShouldReturnAllCustomers_WhenNoFilters()
    {
        var context = GetDbContext();

        context.Customers.AddRange(
            new Customer
            {
                CustomerId = "A",
                CompanyName = "Alpha",
                City = "San Salvador",
                Country = "El Salvador"
            },
            new Customer
            {
                CustomerId = "B",
                CompanyName = "Beta",
                City = "Santa Ana",
                Country = "El Salvador"
            }
        );

        context.SaveChanges();

        var service = new CustomerService(context);

        var result = service.GetCustomerDtos(null, null, null);

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void GetCustomerDtos_ShouldFilterByName()
    {
        var context = GetDbContext();

        context.Customers.AddRange(
            new Customer
            {
                CustomerId = "A",
                CompanyName = "Tech Corp"
            },
            new Customer
            {
                CustomerId = "B",
                CompanyName = "Food Company"
            }
        );

        context.SaveChanges();

        var service = new CustomerService(context);

        var result = service.GetCustomerDtos("Tech", null, null);

        Assert.Single(result);
        Assert.Equal("Tech Corp", result[0].CompanyName);
    }

    [Fact]
    public void GetCustomerDtos_ShouldFilterByCity()
    {
        var context = GetDbContext();

        context.Customers.AddRange(
            new Customer
            {
                CustomerId = "A",
                CompanyName = "Test1",
                City = "San Salvador"
            },
            new Customer
            {
                CustomerId = "B",
                CompanyName = "Test2",
                City = "Santa Ana"
            }
        );

        context.SaveChanges();

        var service = new CustomerService(context);

        var result = service.GetCustomerDtos(null, "San Salvador", null);

        Assert.Single(result);
    }

    [Fact]
    public void GetCustomerDtos_ShouldFilterByCountry()
    {
        var context = GetDbContext();

        context.Customers.AddRange(
            new Customer
            {
                CustomerId = "A",
                CompanyName = "Aristos",
                Country = "El Salvador"
            },
            new Customer
            {
                CustomerId = "B",
                CompanyName = "Guatemala Shippers",
                Country = "Guatemala"
            }
        );

        context.SaveChanges();

        var service = new CustomerService(context);

        var result = service.GetCustomerDtos(null, null, "El Salvador");

        Assert.Single(result);
    }

    [Fact]
    public void GetCustomerDtos_ShouldFilterByMultipleFields()
    {
        var context = GetDbContext();

        context.Customers.AddRange(
            new Customer
            {
                CustomerId = "A",
                CompanyName = "Tech Corp",
                City = "San Salvador",
                Country = "El Salvador"
            },
            new Customer
            {
                CustomerId = "B",
                CompanyName = "Tech Corp",
                City = "Santa Ana",
                Country = "El Salvador"
            }
        );

        context.SaveChanges();

        var service = new CustomerService(context);

        var result = service.GetCustomerDtos(
            "Tech",
            "San Salvador",
            "El Salvador"
        );

        Assert.Single(result);
        Assert.Equal("A", result[0].CustomerId);
    }

    [Fact]
    public void GetCustomerShippingDtos_ShouldReturnShippingInfo()
    {
        var context = GetDbContext();

        context.Customers.Add(new Customer
        {
            CustomerId = "ALFKI",
            CompanyName = "RSM",
            Address = "Street 123",
            City = "San Salvador",
            Region = "SS",
            PostalCode = "1101",
            Country = "El Salvador"
        });

        context.SaveChanges();

        var service = new CustomerService(context);

        var result = service.GetCustomerShippingDtos("ALFKI");

        Assert.Single(result);

        Assert.Equal("Street 123", result[0].Address);
        Assert.Equal("San Salvador", result[0].City);
        Assert.Equal("SS", result[0].Region);
        Assert.Equal("1101", result[0].PostalCode);
        Assert.Equal("El Salvador", result[0].Country);
    }

    [Fact]
    public void GetCustomerShippingDtos_ShouldReturnEmpty_WhenCustomerNotFound()
    {
        var context = GetDbContext();

        var service = new CustomerService(context);

        var result = service.GetCustomerShippingDtos("NOTFOUND");

        Assert.Empty(result);
    }

    [Fact]
    public void GetCustomerShippingDtos_ShouldReturnAll_WhenCustomerIdIsEmpty()
    {
        var context = GetDbContext();

        context.Customers.AddRange(
            new Customer
            {
                CustomerId = "A",
                CompanyName = "Test1"
            },
            new Customer
            {
                CustomerId = "B",
                CompanyName = "Test2"
            }
        );

        context.SaveChanges();

        var service = new CustomerService(context);

        var result = service.GetCustomerShippingDtos("");

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void GetCustomerDtos_ShouldHandleNullFields()
    {
        var context = GetDbContext();

        context.Customers.Add(new Customer
        {
            CustomerId = "A",
            CompanyName = "null",
            City = null,
            Country = null
        });

        context.SaveChanges();

        var service = new CustomerService(context);

        var result = service.GetCustomerDtos("test", "city", "country");

        Assert.Empty(result);
    }
}