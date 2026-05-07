using Xunit;
using Microsoft.EntityFrameworkCore;
using RSM.Application.Services;
using RSM.Infrastructure.Data;
using RSM.Domain.Models;
using RSM.Application.Dtos;

public class OrderDetailServiceTests
{
    private ApplicationDBContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDBContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDBContext(options);
    }

    [Fact]
    public void GetOrderDetail_ShouldReturnOrderWithProductsDto()
    {
        var context = GetDbContext();

        var category = new Category { CategoryId = 1, CategoryName = "Beverages" };
        context.Categories.Add(category);

        var product = new Product
        {
            ProductId = 1,
            ProductName = "Chai",
            Category = category,
            QuantityPerUnit = "10 boxes",
            Discontinued = false,
            UnitsInStock = 39
        };
        context.Products.Add(product);

        var customer = new Customer
        {
            CustomerId = "ALFKI",
            CompanyName = "Alfreds Futterkiste"
        };
        context.Customers.Add(customer);

        context.Orders.Add(new Order
        {
            OrderId = 1,
            CustomerId = "ALFKI",
            Customer = customer,
            OrderDate = new DateTime(2025, 1, 1),
            ShipAddress = "123 Main St",
            ShipCity = "Berlin",
            ShipRegion = "Berlin",
            ShipCountry = "Germany",
            ShipPostalCode = "12209",
            ShipperId = 1,
            Status = 1,
            Freight = 32.38m,
            Latitude = 52.5200m,
            Longitude = 13.4050m,
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

        var service = new OrderDetailService(context);

        var result = service.GetOrderDetail(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.OrderId);
        Assert.Equal("Alfreds Futterkiste", result.CustomerName);
        Assert.Equal("123 Main St", result.ShippingAddress);
        Assert.Equal(32.38m, result.Freight);
    }

    [Fact]
    public void GetOrderDetail_ShouldReturnAllOrderProducts()
    {
        var context = GetDbContext();

        var category = new Category { CategoryId = 1, CategoryName = "Beverages" };
        context.Categories.Add(category);

        var product1 = new Product
        {
            ProductId = 1,
            ProductName = "Chai",
            Category = category,
            QuantityPerUnit = "10 boxes",
            Discontinued = false,
            UnitsInStock = 39
        };

        var product2 = new Product
        {
            ProductId = 2,
            ProductName = "Chang",
            Category = category,
            QuantityPerUnit = "24 sticks",
            Discontinued = false,
            UnitsInStock = 17
        };

        context.Products.AddRange(product1, product2);

        var customer = new Customer { CustomerId = "ALFKI", CompanyName = "Alfreds" };
        context.Customers.Add(customer);

        context.Orders.Add(new Order
        {
            OrderId = 1,
            CustomerId = "ALFKI",
            Customer = customer,
            OrderDetails = new List<OrderDetails>
            {
                new OrderDetails { ProductId = 1, Product = product1, Quantity = 10, UnitPrice = 18, Discount = 0 },
                new OrderDetails { ProductId = 2, Product = product2, Quantity = 20, UnitPrice = 19, Discount = 0 }
            }
        });

        context.SaveChanges();

        var service = new OrderDetailService(context);

        var result = service.GetOrderDetail(1);

        Assert.Equal(2, result.Products.Count);
    }

    [Fact]
    public void GetOrderDetail_ShouldThrow_WhenOrderNotFound()
    {
        var context = GetDbContext();

        var service = new OrderDetailService(context);

        Assert.Throws<Exception>(() => service.GetOrderDetail(999));
    }

    [Fact]
    public void GetOrderDetail_ShouldReturnCorrectOrderProductDto()
    {
        var context = GetDbContext();

        var category = new Category { CategoryId = 1, CategoryName = "Beverages" };
        context.Categories.Add(category);

        var product = new Product
        {
            ProductId = 1,
            ProductName = "Chai",
            Category = category,
            QuantityPerUnit = "10 boxes",
            Discontinued = false,
            UnitsInStock = 39
        };
        context.Products.Add(product);

        var customer = new Customer { CustomerId = "ALFKI", CompanyName = "Alfreds" };
        context.Customers.Add(customer);

        context.Orders.Add(new Order
        {
            OrderId = 1,
            CustomerId = "ALFKI",
            Customer = customer,
            OrderDetails = new List<OrderDetails>
            {
                new OrderDetails
                {
                    ProductId = 1,
                    Product = product,
                    Quantity = 10,
                    UnitPrice = 18,
                    Discount = 0.1f
                }
            }
        });

        context.SaveChanges();

        var service = new OrderDetailService(context);

        var result = service.GetOrderDetail(1);

        var productDto = Assert.Single(result.Products);
        Assert.Equal(1, productDto.ProductId);
        Assert.Equal("Chai", productDto.ProductName);
        Assert.Equal(18, productDto.UnitPrice);
        Assert.Equal(10, productDto.Quantity);
        Assert.Equal(0.1f, productDto.Discount);
    }

    [Fact]
    public void GetOrderDetail_ShouldHandleNullValues()
    {
        var context = GetDbContext();

        var customer = new Customer
        {
            CustomerId = "ALFKI",
            CompanyName = "Alfreds"
        };
        context.Customers.Add(customer);

        context.Orders.Add(new Order
        {
            OrderId = 1,
            CustomerId = "ALFKI",
            Customer = customer,
            ShipAddress = null,
            ShipCity = null,
            ShipRegion = null,
            ShipCountry = null,
            ShipPostalCode = null,
            Freight = null,
            OrderDetails = new List<OrderDetails>()
        });

        context.SaveChanges();

        var service = new OrderDetailService(context);

        var result = service.GetOrderDetail(1);

        Assert.Equal("", result.ShippingAddress);
        Assert.Equal("", result.City);
        Assert.Equal("", result.Region);
        Assert.Equal("", result.Country);
        Assert.Equal("", result.PostalCode);
        Assert.Equal(0, result.Freight);
    }

    [Fact]
    public void GetOrderDetail_ShouldCalculateCorrectValues()
    {
        var context = GetDbContext();

        var category = new Category { CategoryId = 1, CategoryName = "Beverages" };
        context.Categories.Add(category);

        var product = new Product
        {
            ProductId = 1,
            ProductName = "Chai",
            Category = category,
            QuantityPerUnit = "10 boxes",
            Discontinued = false,
            UnitsInStock = 39
        };
        context.Products.Add(product);

        var customer = new Customer { CustomerId = "ALFKI", CompanyName = "Alfreds" };
        context.Customers.Add(customer);

        context.Orders.Add(new Order
        {
            OrderId = 1,
            CustomerId = "ALFKI",
            Customer = customer,
            Freight = 5m,
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

        var service = new OrderDetailService(context);

        var result = service.GetOrderDetail(1);

        // Total = (10 * 18) + 5 freight = 185
        Assert.Equal(185, result.Freight + (10 * 18));
    }

    [Fact]
    public void GetOrderDetail_ShouldHandleDiscountCorrectly()
    {
        var context = GetDbContext();

        var category = new Category { CategoryId = 1, CategoryName = "Beverages" };
        context.Categories.Add(category);

        var product = new Product
        {
            ProductId = 1,
            ProductName = "Chai",
            Category = category,
            QuantityPerUnit = "10 boxes",
            Discontinued = false,
            UnitsInStock = 39
        };
        context.Products.Add(product);

        var customer = new Customer { CustomerId = "ALFKI", CompanyName = "Alfreds" };
        context.Customers.Add(customer);

        context.Orders.Add(new Order
        {
            OrderId = 1,
            CustomerId = "ALFKI",
            Customer = customer,
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

        var service = new OrderDetailService(context);

        var result = service.GetOrderDetail(1);

        var productDto = Assert.Single(result.Products);
        Assert.Equal(0.1f, productDto.Discount);
    }
}
