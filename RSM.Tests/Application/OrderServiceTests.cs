using Xunit;
using Microsoft.EntityFrameworkCore;
using Moq;
using RSM.Application.Services;
using RSM.Application.Dtos;
using RSM.Infrastructure.Data;
using RSM.Domain.Models;
using RSM.Application.Services.External;
using System.Globalization;

public class OrderServiceTests
{
    private ApplicationDBContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDBContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDBContext(options);
    }

    [Fact]
    public void GetOrderDetail_ShouldReturnOrder_WhenExists()
    {
        var context = GetDbContext();

        context.Orders.Add(new Order
        {
            OrderId = 1,
            Freight = 10,
            Customer = new Customer 
            { 
                CustomerId = "ALFKI",
                CompanyName = "Test" 
            },
            OrderDetails = new List<OrderDetails>()
        });

        context.SaveChanges();

        var mockGoogle = new Mock<IGoogleAddressService>();

        var service = new OrderService(context, mockGoogle.Object);

        var result = service.GetOrderDetail(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.OrderId);
        Assert.Equal(10, result.Freight);
    }

    [Fact]
    public void GetOrderDetail_ShouldThrow_WhenNotFound()
    {
        var context = GetDbContext();
        var mockGoogle = new Mock<IGoogleAddressService>();

        var service = new OrderService(context, mockGoogle.Object);

        Assert.Throws<Exception>(() =>
            service.GetOrderDetail(999)
        );
    }

    [Fact]
    public async Task UpdateOrderStatus_ShouldUpdate_WhenValidTransition()
    {
        var context = GetDbContext();

        context.Orders.Add(new Order
        {
            OrderId = 1,
            Status = 0
        });

        context.SaveChanges();
        var mockGoogle = new Mock<IGoogleAddressService>();

        var service = new OrderService(context, mockGoogle.Object);

        await service.UpdateOrderStatusAsync(1, 1);

        var order = context.Orders.First();

        Assert.Equal(1, order.Status);
    }

    [Fact]
    public async Task UpdateOrderStatus_ShouldThrow_WhenInvalidTransition()
    {
        var context = GetDbContext();

        context.Orders.Add(new Order
        {
            OrderId = 1,
            Status = 3
        });

        context.SaveChanges();
        var mockGoogle = new Mock<IGoogleAddressService>();

        var service = new OrderService(context, mockGoogle.Object);

        await Assert.ThrowsAsync<Exception>(() =>
            service.UpdateOrderStatusAsync(1, 1)
        );
    }

    [Fact]
    public void GetOrders_ShouldFilterByYear()
    {
        var context = GetDbContext();

        context.Orders.Add(new Order
        {
            OrderId = 1,
            OrderDate = new DateTime(2025, 1, 1),
            ShipCountry = "El Salvador",
            Status = 1,
            Customer = new Customer { CustomerId = "A", CompanyName = "Test" },
            OrderDetails = new List<OrderDetails>()
        });

        context.SaveChanges();

        var service = new OrderService(context, new Mock<IGoogleAddressService>().Object);

        var result = service.GetOrders(2025, null, null, null, null);

        Assert.Single(result);
    }

    [Fact]
    public void GetOrderDetail_ShouldCalculateTotalCorrectly()
    {
        var context = GetDbContext();

        context.Products.Add(new Product
        {
            ProductId = 1,
            ProductName = "Test Product",
            UnitPrice = 10
        });

        context.Orders.Add(new Order
        {
            OrderId = 1,
            Freight = 5,
            Customer = new Customer
            {
                CustomerId = "A",
                CompanyName = "Test"
            },
            OrderDetails = new List<OrderDetails>
            {
                new OrderDetails
                {
                    ProductId = 1,
                    Quantity = 2,
                    UnitPrice = 10,
                    Discount = 0
                }
            }
        });

        context.SaveChanges();

        var service = new OrderService(context, new Mock<IGoogleAddressService>().Object);

        var result = service.GetOrderDetail(1);

        Assert.Equal(25, result.Total);
    }

    [Fact]
    public void GetOrderDetail_ShouldThrow_WhenOrderNotFound()
    {
        var context = GetDbContext();

        var service = new OrderService(context, new Mock<IGoogleAddressService>().Object);

        Assert.Throws<Exception>(() => service.GetOrderDetail(999));
    }

    [Fact]
    public void GetOrderYears_ShouldReturnDistinctYears()
    {
        var context = GetDbContext();

        context.Orders.AddRange(
            new Order { OrderDate = new DateTime(2024, 1, 1) },
            new Order { OrderDate = new DateTime(2025, 1, 1) }
        );

        context.SaveChanges();
        var mockGoogle = new Mock<IGoogleAddressService>();

        var service = new OrderService(context, mockGoogle.Object);

        var result = service.GetOrderYears();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task UpdateOrderStatus_ShouldThrow_InvalidTransition()
    {
        var context = GetDbContext();

        context.Orders.Add(new Order
        {
            OrderId = 1,
            Status = 3
        });

        context.SaveChanges();
        var mockGoogle = new Mock<IGoogleAddressService>();

        var service = new OrderService(context, mockGoogle.Object);

        await Assert.ThrowsAsync<Exception>(() =>
            service.UpdateOrderStatusAsync(1, 1)
        );
    }

    [Fact]
    public async Task UpdateOrderStatus_ShouldThrow_WhenOrderNotFound()
    {
        var context = GetDbContext();
        var mockGoogle = new Mock<IGoogleAddressService>();
        var service = new OrderService(context, mockGoogle.Object);

        await Assert.ThrowsAsync<Exception>(() =>
            service.UpdateOrderStatusAsync(999, 1)
        );
    }

    [Fact]
    public async Task DeleteOrder_ShouldThrow_WhenNotPending()
    {
        var context = GetDbContext();

        context.Orders.Add(new Order
        {
            OrderId = 1,
            Status = 2
        });

        context.SaveChanges();
        var mockGoogle = new Mock<IGoogleAddressService>();
        var service = new OrderService(context, mockGoogle.Object);

        await Assert.ThrowsAsync<Exception>(() =>
            service.DeleteOrderAsync(1)
        );
    }

    [Fact]
    public async Task DeleteOrder_ShouldReturnTrue_WhenDeleted()
    {
        var context = GetDbContext();

        context.Orders.Add(new Order
        {
            OrderId = 1,
            Status = 0,
            OrderDetails = new List<OrderDetails>()
        });

        context.SaveChanges();
        var mockGoogle = new Mock<IGoogleAddressService>();

        var service = new OrderService(context, mockGoogle.Object);

        var result = await service.DeleteOrderAsync(1);

        Assert.True(result);
    }

    [Fact]
    public void GetOrders_ShouldFilterByCountry()
    {
        var context = GetDbContext();

        var customer = new Customer
        {
            CustomerId = "A",
            CompanyName = "Test"
        };

        context.Orders.Add(new Order
        {
            OrderId = 1,
            ShipCountry = "El Salvador",
            OrderDate = new DateTime(2025, 1, 1),
            Status = 1,
            Customer = customer,
            OrderDetails = new List<OrderDetails>()
        });

        context.Orders.Add(new Order
        {
            OrderId = 2,
            ShipCountry = "USA",
            OrderDate = new DateTime(2025, 1, 1),
            Status = 1,
            Customer = customer,
            OrderDetails = new List<OrderDetails>()
        });

        context.SaveChanges();
        var service = new OrderService(context, new Mock<IGoogleAddressService>().Object);

        var result = service.GetOrders(null, null, null, "El Salvador", null);

        Assert.Single(result);
        
    }

    [Fact]
    public void GetOrders_ShouldFilterByStatus()
    {
        var context = GetDbContext();

        context.Orders.Add(new Order
        {
            OrderId = 1,
            Status = 1,
            Customer = new Customer {CustomerId = "TestA", CompanyName = "A" },
            OrderDetails = new List<OrderDetails>()
        });

        context.Orders.Add(new Order
        {
            OrderId = 2,
            Status = 2,
            Customer = new Customer { CustomerId = "TestB", CompanyName = "B" },
            OrderDetails = new List<OrderDetails>()
        });

        context.SaveChanges();

        var service = new OrderService(context, new Mock<IGoogleAddressService>().Object);

        var result = service.GetOrders(null, null, null, null, 1);

        Assert.Single(result);
    }

    [Fact]
    public async Task UpdateOrder_ShouldThrow_WhenOrderNotFound()
    {
        var context = GetDbContext();

        var mockGoogle = new Mock<IGoogleAddressService>();

        var service = new OrderService(context, mockGoogle.Object);

        var dto = new UpdateOrderDto
        {
            OrderId = 1,
            ShipperId = 1,
            Products = new List<UpdateOrderDetailDto>()
        };

        await Assert.ThrowsAsync<Exception>(() =>
            service.UpdateOrderAsync(dto)
        );
    }

    [Fact]
    public async Task UpdateOrder_ShouldThrow_WhenStatusGreaterThan1()
    {
        var context = GetDbContext();

        context.Orders.Add(new Order
        {
            OrderId = 1,
            Status = 2
        });

        context.SaveChanges();

        var mockGoogle = new Mock<IGoogleAddressService>();

        var service = new OrderService(context, mockGoogle.Object);

        var dto = new UpdateOrderDto
        {
            OrderId = 1,
            ShipperId = 1,
            Products = new List<UpdateOrderDetailDto>()
        };

        await Assert.ThrowsAsync<Exception>(() =>
            service.UpdateOrderAsync(dto)
        );
    }

    [Fact]
    public async Task CreateOrder_ShouldThrow_WhenNoProducts()
    {
        var context = GetDbContext();

        var mockGoogle = new Mock<IGoogleAddressService>();

        var service = new OrderService(context, mockGoogle.Object);

        var dto = new CreateOrderDto
        {
            Products = new List<UpdateOrderDetailDto>()
        };

        await Assert.ThrowsAsync<Exception>(() =>
            service.CreateOrderAsync(dto)
        );
    }

    [Fact]
    public async Task CreateOrder_ShouldThrow_WhenInvalidAddress()
    {
        var context = GetDbContext();

        var mockGoogle = new Mock<IGoogleAddressService>();

        mockGoogle
            .Setup(x => x.ValidateAddressAsync(It.IsAny<AddressDto>()))
            .ReturnsAsync(new AddressValidationResult
            {
                IsValid = false
            });

        var service = new OrderService(context, mockGoogle.Object);

        var dto = new CreateOrderDto
        {
            Products = new List<UpdateOrderDetailDto>
            {
                new UpdateOrderDetailDto { ProductId = 1, Quantity = 1, Discount = 0 }
            }
        };

        await Assert.ThrowsAsync<Exception>(() =>
            service.CreateOrderAsync(dto)
        );
    }

    [Fact]
    public void GetOrderYearsByCountryAndYear_ShouldReturnGroupedCountries()
    {
        var context = GetDbContext();

        context.Orders.AddRange(
            new Order
            {
                OrderDate = new DateTime(2025, 1, 1),
                ShipCountry = "USA"
            },
            new Order
            {
                OrderDate = new DateTime(2025, 2, 1),
                ShipCountry = "USA"
            },
            new Order
            {
                OrderDate = new DateTime(2025, 3, 1),
                ShipCountry = "Canada"
            }
        );

        context.SaveChanges();

        var service = new OrderService(
            context,
            new Mock<IGoogleAddressService>().Object
        );

        var result = service.GetOrderYearsByCountryAndYear(2025);

        Assert.Equal(2, result.Count);

        Assert.Equal("USA", result[0].Country);
        Assert.Equal(2, result[0].Total);
    }

    [Fact]
    public void GetOrdersByMonth_ShouldReturnMonthlyTotals()
    {
        var context = GetDbContext();

        context.Orders.AddRange(
            new Order
            {
                OrderDate = new DateTime(2025, 1, 1)
            },
            new Order
            {
                OrderDate = new DateTime(2025, 1, 15)
            },
            new Order
            {
                OrderDate = new DateTime(2025, 2, 1)
            }
        );

        context.SaveChanges();

        var service = new OrderService(
            context,
            new Mock<IGoogleAddressService>().Object
        );

        var result = service.GetOrdersByMonth(2025);

        Assert.Equal(2, result.Count);

        Assert.Equal(1, result[0].Month);
        Assert.Equal(2, result[0].Total);
    }

    [Fact]
    public void GetOrders_ShouldFilterByMonth()
    {
        var context = GetDbContext();

        context.Orders.AddRange(
            new Order
            {
                OrderId = 1,
                OrderDate = new DateTime(2025, 1, 1),
                ShipCountry = "USA",
                Status = 1,
                Customer = new Customer
                {
                    CustomerId = "A",
                    CompanyName = "Test"
                },
                OrderDetails = new List<OrderDetails>()
            },
            new Order
            {
                OrderId = 2,
                OrderDate = new DateTime(2025, 2, 1),
                ShipCountry = "USA",
                Status = 1,
                Customer = new Customer
                {
                    CustomerId = "B",
                    CompanyName = "Test2"
                },
                OrderDetails = new List<OrderDetails>()
            }
        );

        context.SaveChanges();

        var service = new OrderService(
            context,
            new Mock<IGoogleAddressService>().Object
        );

        var result = service.GetOrders(2025, 1, null, null, null);

        Assert.Single(result);
        Assert.Equal(1, result[0].OrderId);
    }

    [Fact]
    public void GetOrders_ShouldFilterByWeek()
    {
        var context = GetDbContext();

        context.Orders.Add(new Order
        {
            OrderId = 1,
            OrderDate = new DateTime(2025, 1, 1),
            ShipCountry = "USA",
            Status = 1,
            Customer = new Customer
            {
                CustomerId = "A",
                CompanyName = "Test"
            },
            OrderDetails = new List<OrderDetails>()
        });

        context.SaveChanges();

        var service = new OrderService(
            context,
            new Mock<IGoogleAddressService>().Object
        );

        var week = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(
            new DateTime(2025, 1, 1),
            CalendarWeekRule.FirstDay,
            DayOfWeek.Monday
        );

        var result = service.GetOrders(2025, null, week, null, null);

        Assert.Single(result);
    }

    [Fact]
    public void GetOrders_ShouldReturnPendingStatus()
    {
        var context = GetDbContext();

        context.Orders.Add(new Order
        {
            OrderId = 1,
            Status = 0,
            Customer = new Customer
            {
                CustomerId = "A",
                CompanyName = "Test"
            },
            OrderDetails = new List<OrderDetails>()
        });

        context.SaveChanges();

        var service = new OrderService(
            context,
            new Mock<IGoogleAddressService>().Object
        );

        var result = service.GetOrders(null, null, null, null, null);

        Assert.Equal("Pending", result.First().Status);
    }

    [Fact]
    public void GetOrders_ShouldReturnShippedStatus()
    {
        var context = GetDbContext();

        context.Orders.Add(new Order
        {
            OrderId = 1,
            Status = 2,
            Customer = new Customer
            {
                CustomerId = "A",
                CompanyName = "Test"
            },
            OrderDetails = new List<OrderDetails>()
        });

        context.SaveChanges();

        var service = new OrderService(
            context,
            new Mock<IGoogleAddressService>().Object
        );

        var result = service.GetOrders(null, null, null, null, null);

        Assert.Equal("Shipped", result.First().Status);
    }

    [Fact]
    public void GetOrders_ShouldReturnCompletedStatus()
    {
        var context = GetDbContext();

        context.Orders.Add(new Order
        {
            OrderId = 1,
            Status = 3,
            Customer = new Customer
            {
                CustomerId = "A",
                CompanyName = "Test"
            },
            OrderDetails = new List<OrderDetails>()
        });

        context.SaveChanges();

        var service = new OrderService(
            context,
            new Mock<IGoogleAddressService>().Object
        );

        var result = service.GetOrders(null, null, null, null, null);

        Assert.Equal("Completed", result.First().Status);
    }

    [Fact]
    public void GetOrders_ShouldReturnCancelledStatus()
    {
        var context = GetDbContext();

        context.Orders.Add(new Order
        {
            OrderId = 1,
            Status = 4,
            Customer = new Customer
            {
                CustomerId = "A",
                CompanyName = "Test"
            },
            OrderDetails = new List<OrderDetails>()
        });

        context.SaveChanges();

        var service = new OrderService(
            context,
            new Mock<IGoogleAddressService>().Object
        );

        var result = service.GetOrders(null, null, null, null, null);

        Assert.Equal("Cancelled", result.First().Status);
    }

    [Fact]
    public void GetOrderDetail_ShouldReturnProcessedStatus()
    {
        var context = GetDbContext();

        context.Orders.Add(new Order
        {
            OrderId = 1,
            Status = 1,
            Customer = new Customer
            {
                CustomerId = "A",
                CompanyName = "Test"
            },
            OrderDetails = new List<OrderDetails>()
        });

        context.SaveChanges();

        var service = new OrderService(
            context,
            new Mock<IGoogleAddressService>().Object
        );

        var result = service.GetOrderDetail(1);

        Assert.Equal("Processed", result.Status);
    }

    [Fact]
    public void GetOrdersByMonth_ShouldReturnCorrectTotals()
    {
        var context = GetDbContext();

        context.Orders.AddRange(
            new Order
            {
                OrderDate = new DateTime(2025, 1, 1)
            },
            new Order
            {
                OrderDate = new DateTime(2025, 1, 15)
            },
            new Order
            {
                OrderDate = new DateTime(2025, 2, 1)
            }
        );

        context.SaveChanges();

        var service = new OrderService(
            context,
            new Mock<IGoogleAddressService>().Object
        );

        var result = service.GetOrdersByMonth(2025);

        Assert.Equal(2, result.Count);

        Assert.Equal(1, result[0].Month);
        Assert.Equal(2, result[0].Total);

        Assert.Equal(2, result[1].Month);
        Assert.Equal(1, result[1].Total);
    }

    [Fact]
    public async Task UpdateOrderAsync_ShouldThrow_WhenOrderNotFound()
    {
        var context = GetDbContext();

        var googleMock = new Mock<IGoogleAddressService>();

        var service = new OrderService(context, googleMock.Object);

        var dto = new UpdateOrderDto
        {
            OrderId = 999,
            ShipperId = 1,
            ShippingAddress = "Street",
            City = "City",
            Region = "Region",
            Country = "Country",
            PostalCode = "0000",
            Freight = 10,
            Status = 1,
            Products = new List<UpdateOrderDetailDto>()
        };

        var ex = await Assert.ThrowsAsync<Exception>(() =>
            service.UpdateOrderAsync(dto));

        Assert.Equal("Order not found", ex.Message);
    }

    [Fact]
    public async Task UpdateOrderAsync_ShouldThrow_WhenStatusGreaterThan1()
    {
        var context = GetDbContext();

        context.Orders.Add(new Order
        {
            OrderId = 1,
            Status = 2
        });

        context.SaveChanges();

        var googleMock = new Mock<IGoogleAddressService>();

        var service = new OrderService(context, googleMock.Object);

        var dto = new UpdateOrderDto
        {
            OrderId = 1,
            ShipperId = 1,
            ShippingAddress = "Street",
            City = "City",
            Region = "Region",
            Country = "Country",
            PostalCode = "0000",
            Freight = 10,
            Status = 1,
            Products = new List<UpdateOrderDetailDto>()
        };

        var ex = await Assert.ThrowsAsync<Exception>(() =>
            service.UpdateOrderAsync(dto));

        Assert.Equal("Order cannot be edited", ex.Message);
    }

    [Fact]
    public async Task UpdateOrderAsync_ShouldThrow_WhenAddressInvalid()
    {
        var context = GetDbContext();

        context.Orders.Add(new Order
        {
            OrderId = 1,
            Status = 0,
            OrderDetails = new List<OrderDetails>()
        });

        context.SaveChanges();

        var googleMock = new Mock<IGoogleAddressService>();

        googleMock.Setup(g => g.ValidateAddressAsync(It.IsAny<AddressDto>()))
            .ReturnsAsync(new AddressValidationResult
            {
                IsValid = false
            });

        var service = new OrderService(context, googleMock.Object);

        var dto = new UpdateOrderDto
        {
            OrderId = 1,
            ShipperId = 1,
            ShippingAddress = "Street",
            City = "City",
            Region = "Region",
            Country = "Country",
            PostalCode = "0000",
            Freight = 10,
            Status = 1,
            Products = new List<UpdateOrderDetailDto>()
        };

        var ex = await Assert.ThrowsAsync<Exception>(() =>
            service.UpdateOrderAsync(dto));

        Assert.Equal("Invalid shipping address", ex.Message);
    }

    [Fact]
    public async Task CreateOrderAsync_ShouldThrow_WhenAddressInvalid()
    {
        var context = GetDbContext();

        var googleMock = new Mock<IGoogleAddressService>();

        googleMock.Setup(g => g.ValidateAddressAsync(It.IsAny<AddressDto>()))
            .ReturnsAsync(new AddressValidationResult
            {
                IsValid = false
            });

        var service = new OrderService(context, googleMock.Object);

        var dto = new CreateOrderDto
        {
            CustomerId = "ALFKI",
            EmployeeId = 1,
            ShipperId = 1,
            ShippingAddress = "Street",
            City = "City",
            Region = "Region",
            Country = "Country",
            PostalCode = "0000",

            Products = new List<UpdateOrderDetailDto>
            {
                new UpdateOrderDetailDto
                {
                    ProductId = 1,
                    Quantity = 1,
                    Discount = 0
                }
            }
        };

        var ex = await Assert.ThrowsAsync<Exception>(() =>
            service.CreateOrderAsync(dto));

        Assert.Equal("Invalid shipping address", ex.Message);
    }

    [Fact]
    public async Task CreateOrderAsync_ShouldThrow_WhenProductNotFound()
    {
        var context = GetDbContext();

        var googleMock = new Mock<IGoogleAddressService>();

        googleMock.Setup(g => g.ValidateAddressAsync(It.IsAny<AddressDto>()))
            .ReturnsAsync(new AddressValidationResult
            {
                IsValid = true,
                Latitude = 1,
                Longitude = 1
            });

        var service = new OrderService(context, googleMock.Object);

        var dto = new CreateOrderDto
        {
            CustomerId = "ALFKI",
            EmployeeId = 1,
            ShipperId = 1,
            ShippingAddress = "Street",
            City = "City",
            Region = "Region",
            Country = "Country",
            PostalCode = "0000",

            Products = new List<UpdateOrderDetailDto>
            {
                new UpdateOrderDetailDto
                {
                    ProductId = 999,
                    Quantity = 1,
                    Discount = 0
                }
            }
        };

        var ex = await Assert.ThrowsAsync<Exception>(() =>
            service.CreateOrderAsync(dto));

        Assert.Contains("Product 999 not found", ex.Message);
    }

    [Fact]
    public async Task CreateOrderAsync_ShouldCreateOrderSuccessfully()
    {
        var context = GetDbContext();

        context.Products.Add(new Product
        {
            ProductId = 1,
            ProductName = "Chai",
            UnitsInStock = 20,
            UnitPrice = 10,
            Discontinued = false
        });

        context.SaveChanges();

        var googleMock = new Mock<IGoogleAddressService>();

        googleMock.Setup(g => g.ValidateAddressAsync(It.IsAny<AddressDto>()))
            .ReturnsAsync(new AddressValidationResult
            {
                IsValid = true,
                Latitude = 10,
                Longitude = 20
            });

        var service = new OrderService(context, googleMock.Object);

        var dto = new CreateOrderDto
        {
            CustomerId = "ALFKI",
            EmployeeId = 1,
            ShipperId = 1,
            ShipName = "Test",
            ShippingAddress = "Street",
            City = "City",
            Region = "Region",
            Country = "Country",
            PostalCode = "0000",
            Freight = 10,

            Products = new List<UpdateOrderDetailDto>
            {
                new UpdateOrderDetailDto
                {
                    ProductId = 1,
                    Quantity = 2,
                    Discount = 0
                }
            }
        };

        await service.CreateOrderAsync(dto);

        Assert.Single(context.Orders);

        var product = context.Products.First();

        Assert.Equal((short)18, product.UnitsInStock);
    }

    [Fact]
    public async Task DeleteOrderAsync_ShouldRestoreProductStock()
    {
        var context = GetDbContext();

        context.Products.Add(new Product
        {
            ProductId = 1,
            ProductName = "Chai",
            UnitsInStock = 5
        });

        context.Orders.Add(new Order
        {
            OrderId = 1,
            Status = 0,
            OrderDetails = new List<OrderDetails>
            {
                new OrderDetails
                {
                    ProductId = 1,
                    Quantity = 2
                }
            }
        });

        context.SaveChanges();

        var googleMock = new Mock<IGoogleAddressService>();

        var service = new OrderService(context, googleMock.Object);

        await service.DeleteOrderAsync(1);

        var product = context.Products.First();

        Assert.Equal((short)7, product.UnitsInStock);
    }

    
}