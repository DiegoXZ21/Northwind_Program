using Xunit;
using Microsoft.EntityFrameworkCore;
using RSM.Application.Services;
using RSM.Infrastructure.Data;
using RSM.Domain.Models;
using RSM.Application.Dtos;

public class ReportServiceTests
{
    private ApplicationDBContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDBContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDBContext(options);
    }

    [Fact]
    public void GetOrdersForExport_ShouldReturnAllOrders_WhenNoFilters()
    {
        var context = GetDbContext();

        var customer = new Customer { CustomerId = "ALFKI", CompanyName = "Alfreds" };
        context.Customers.Add(customer);

        context.Orders.AddRange(
            new Order
            {
                OrderId = 1,
                CustomerId = "ALFKI",
                Customer = customer,
                OrderDate = new DateTime(2025, 1, 1),
                ShipCountry = "Germany",
                Status = 1,
                OrderDetails = new List<OrderDetails>()
            },
            new Order
            {
                OrderId = 2,
                CustomerId = "ALFKI",
                Customer = customer,
                OrderDate = new DateTime(2025, 2, 1),
                ShipCountry = "Germany",
                Status = 2,
                OrderDetails = new List<OrderDetails>()
            }
        );

        context.SaveChanges();

        var service = new ReportService(context);

        var result = service.GetOrdersForExport(new ExportOrdersRequestDto());

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void GetOrdersForExport_ShouldFilterByOrderIds()
    {
        var context = GetDbContext();

        var customer = new Customer { CustomerId = "ALFKI", CompanyName = "Alfreds" };
        context.Customers.Add(customer);

        context.Orders.AddRange(
            new Order
            {
                OrderId = 1,
                CustomerId = "ALFKI",
                Customer = customer,
                OrderDate = new DateTime(2025, 1, 1),
                ShipCountry = "Germany",
                Status = 1,
                OrderDetails = new List<OrderDetails>()
            },
            new Order
            {
                OrderId = 2,
                CustomerId = "ALFKI",
                Customer = customer,
                OrderDate = new DateTime(2025, 2, 1),
                ShipCountry = "Germany",
                Status = 2,
                OrderDetails = new List<OrderDetails>()
            }
        );

        context.SaveChanges();

        var service = new ReportService(context);

        var result = service.GetOrdersForExport(new ExportOrdersRequestDto { OrderIds = new List<int> { 1 } });

        Assert.Single(result);
        Assert.Equal(1, result[0].OrderId);
    }

    [Fact]
    public void GetOrdersForExport_ShouldFilterByYear()
    {
        var context = GetDbContext();

        var customer = new Customer { CustomerId = "ALFKI", CompanyName = "Alfreds" };
        context.Customers.Add(customer);

        context.Orders.AddRange(
            new Order
            {
                OrderId = 1,
                CustomerId = "ALFKI",
                Customer = customer,
                OrderDate = new DateTime(2024, 1, 1),
                ShipCountry = "Germany",
                Status = 1,
                OrderDetails = new List<OrderDetails>()
            },
            new Order
            {
                OrderId = 2,
                CustomerId = "ALFKI",
                Customer = customer,
                OrderDate = new DateTime(2025, 2, 1),
                ShipCountry = "Germany",
                Status = 2,
                OrderDetails = new List<OrderDetails>()
            }
        );

        context.SaveChanges();

        var service = new ReportService(context);

        var result = service.GetOrdersForExport(new ExportOrdersRequestDto { Year = 2025 });

        Assert.Single(result);
        Assert.Equal(2, result[0].OrderId);
    }

    [Fact]
    public void GetOrdersForExport_ShouldFilterByMonth()
    {
        var context = GetDbContext();

        var customer = new Customer { CustomerId = "ALFKI", CompanyName = "Alfreds" };
        context.Customers.Add(customer);

        context.Orders.AddRange(
            new Order
            {
                OrderId = 1,
                CustomerId = "ALFKI",
                Customer = customer,
                OrderDate = new DateTime(2025, 1, 15),
                ShipCountry = "Germany",
                Status = 1,
                OrderDetails = new List<OrderDetails>()
            },
            new Order
            {
                OrderId = 2,
                CustomerId = "ALFKI",
                Customer = customer,
                OrderDate = new DateTime(2025, 2, 20),
                ShipCountry = "Germany",
                Status = 2,
                OrderDetails = new List<OrderDetails>()
            }
        );

        context.SaveChanges();

        var service = new ReportService(context);

        var result = service.GetOrdersForExport(new ExportOrdersRequestDto { Month = 2 });

        Assert.Single(result);
        Assert.Equal(2, result[0].OrderId);
    }

    [Fact]
    public void GetOrdersForExport_ShouldFilterByCountry()
    {
        var context = GetDbContext();

        var customer = new Customer { CustomerId = "ALFKI", CompanyName = "Alfreds" };
        context.Customers.Add(customer);

        context.Orders.AddRange(
            new Order
            {
                OrderId = 1,
                CustomerId = "ALFKI",
                Customer = customer,
                OrderDate = new DateTime(2025, 1, 1),
                ShipCountry = "Germany",
                Status = 1,
                OrderDetails = new List<OrderDetails>()
            },
            new Order
            {
                OrderId = 2,
                CustomerId = "ALFKI",
                Customer = customer,
                OrderDate = new DateTime(2025, 2, 1),
                ShipCountry = "USA",
                Status = 2,
                OrderDetails = new List<OrderDetails>()
            }
        );

        context.SaveChanges();

        var service = new ReportService(context);

        var result = service.GetOrdersForExport(new ExportOrdersRequestDto { Country = "Germany" });

        Assert.Single(result);
        Assert.Equal("Germany", result[0].Country);
    }

    [Fact]
    public void GetOrdersForExport_ShouldCombineMultipleFilters()
    {
        var context = GetDbContext();

        var customer = new Customer { CustomerId = "ALFKI", CompanyName = "Alfreds" };
        context.Customers.Add(customer);

        context.Orders.AddRange(
            new Order
            {
                OrderId = 1,
                CustomerId = "ALFKI",
                Customer = customer,
                OrderDate = new DateTime(2025, 1, 15),
                ShipCountry = "Germany",
                Status = 1,
                OrderDetails = new List<OrderDetails>()
            },
            new Order
            {
                OrderId = 2,
                CustomerId = "ALFKI",
                Customer = customer,
                OrderDate = new DateTime(2025, 2, 20),
                ShipCountry = "Germany",
                Status = 2,
                OrderDetails = new List<OrderDetails>()
            },
            new Order
            {
                OrderId = 3,
                CustomerId = "ALFKI",
                Customer = customer,
                OrderDate = new DateTime(2024, 1, 10),
                ShipCountry = "Germany",
                Status = 1,
                OrderDetails = new List<OrderDetails>()
            }
        );

        context.SaveChanges();

        var service = new ReportService(context);

        var result = service.GetOrdersForExport(new ExportOrdersRequestDto
        {
            Year = 2025,
            Month = 1,
            Country = "Germany"
        });

        Assert.Single(result);
        Assert.Equal(1, result[0].OrderId);
    }

    [Fact]
    public void GetOrdersForExport_ShouldReturnCorrectOrderListDto()
    {
        var context = GetDbContext();

        var category = new Category { CategoryId = 1, CategoryName = "Beverages" };
        context.Categories.Add(category);

        var product = new Product
        {
            ProductId = 1,
            ProductName = "Chai",
            Category = category,
            UnitPrice = 18
        };
        context.Products.Add(product);

        var customer = new Customer { CustomerId = "ALFKI", CompanyName = "Alfreds Futterkiste" };
        context.Customers.Add(customer);

        context.Orders.Add(new Order
        {
            OrderId = 1,
            CustomerId = "ALFKI",
            Customer = customer,
            OrderDate = new DateTime(2025, 1, 15),
            ShipCountry = "Germany",
            Status = 1,
            OrderDetails = new List<OrderDetails>
            {
                new OrderDetails
                {
                    ProductId = 1,
                    Product = product,
                    Quantity = 10,
                    UnitPrice = 18,
                    Discount = 0
                }
            }
        });

        context.SaveChanges();

        var service = new ReportService(context);

        var result = service.GetOrdersForExport(new ExportOrdersRequestDto());

        var dto = Assert.Single(result);
        Assert.IsType<OrderListDto>(dto);
        Assert.Equal(1, dto.OrderId);
        Assert.Equal("Alfreds Futterkiste", dto.CustomerName);
        Assert.Equal("Germany", dto.Country);
        Assert.Equal("Processed", dto.Status);
    }

    [Fact]
    public void GetOrdersForExport_ShouldCalculateTotalAmount()
    {
        var context = GetDbContext();

        var category = new Category { CategoryId = 1, CategoryName = "Beverages" };
        context.Categories.Add(category);

        var product = new Product
        {
            ProductId = 1,
            ProductName = "Chai",
            Category = category,
            UnitPrice = 20
        };
        context.Products.Add(product);

        var customer = new Customer { CustomerId = "ALFKI", CompanyName = "Alfreds" };
        context.Customers.Add(customer);

        context.Orders.Add(new Order
        {
            OrderId = 1,
            CustomerId = "ALFKI",
            Customer = customer,
            OrderDate = new DateTime(2025, 1, 1),
            ShipCountry = "Germany",
            Status = 1,
            OrderDetails = new List<OrderDetails>
            {
                new OrderDetails
                {
                    ProductId = 1,
                    Product = product,
                    Quantity = 10,
                    UnitPrice = 20,
                    Discount = 0.1f  // 10% discount
                }
            }
        });

        context.SaveChanges();

        var service = new ReportService(context);

        var result = service.GetOrdersForExport(new ExportOrdersRequestDto());

        var dto = Assert.Single(result);
        // Total = 20 * 10 * (1 - 0.1) = 180
        Assert.Equal(180, dto.TotalAmount);
    }

    [Fact]
    public void GetOrdersForExport_ShouldReturnCorrectProductCount()
    {
        var context = GetDbContext();

        var category = new Category { CategoryId = 1, CategoryName = "Beverages" };
        context.Categories.Add(category);

        var product1 = new Product { ProductId = 1, ProductName = "Chai", Category = category, UnitPrice = 18 };
        var product2 = new Product { ProductId = 2, ProductName = "Chang", Category = category, UnitPrice = 19 };
        context.Products.AddRange(product1, product2);

        var customer = new Customer { CustomerId = "ALFKI", CompanyName = "Alfreds" };
        context.Customers.Add(customer);

        context.Orders.Add(new Order
        {
            OrderId = 1,
            CustomerId = "ALFKI",
            Customer = customer,
            OrderDate = new DateTime(2025, 1, 1),
            ShipCountry = "Germany",
            Status = 1,
            OrderDetails = new List<OrderDetails>
            {
                new OrderDetails { ProductId = 1, Product = product1, Quantity = 10, UnitPrice = 18, Discount = 0 },
                new OrderDetails { ProductId = 2, Product = product2, Quantity = 5, UnitPrice = 19, Discount = 0 }
            }
        });

        context.SaveChanges();

        var service = new ReportService(context);

        var result = service.GetOrdersForExport(new ExportOrdersRequestDto());

        var dto = Assert.Single(result);
        Assert.Equal(2, dto.ProductCount);
    }

    [Fact]
    public void GetOrdersForExport_ShouldReturnEmptyList_WhenNoMatches()
    {
        var context = GetDbContext();

        var service = new ReportService(context);

        var result = service.GetOrdersForExport(new ExportOrdersRequestDto { Year = 2099 });

        Assert.Empty(result);
    }
}
