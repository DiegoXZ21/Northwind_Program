using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using RSM.Backend.Controllers;
using RSM.Application.Services;
using RSM.Application.Dtos;

public class OrderControllerTests
{
    [Fact]
    public void GetOrders_ShouldReturnOk()
    {
        var mockService = new Mock<IOrderService>();

        mockService.Setup(s => s.GetOrders(null, null, null, null, null))
            .Returns(new List<OrderListDto>());

        var controller = new OrderController(mockService.Object);

        var result = controller.GetOrders(null, null, null, null, null);

        Assert.IsType<OkObjectResult>(result);
    }


    [Fact]
    public void GetOrderDetail_ShouldReturnOk()
    {
        var mockService = new Mock<IOrderService>();

        mockService.Setup(s => s.GetOrderDetail(1))
            .Returns(new OrderDetailDto { OrderId = 1 });

        var controller = new OrderController(mockService.Object);

        var result = controller.GetOrderDetail(1);

        var ok = Assert.IsType<OkObjectResult>(result);
        var data = Assert.IsType<OrderDetailDto>(ok.Value);

        Assert.Equal(1, data.OrderId);
    }

    [Fact]
    public void GetYears_ShouldReturnOk()
    {
        var mockService = new Mock<IOrderService>();

        mockService.Setup(s => s.GetOrderYears())
            .Returns(new List<int> { 2024, 2025 });

        var controller = new OrderController(mockService.Object);

        var result = controller.GetYears();

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public void GetYears_ShouldReturnBadRequest_WhenException()
    {
        var mockService = new Mock<IOrderService>();

        mockService.Setup(s => s.GetOrderYears())
            .Throws(new Exception("DB error"));

        var controller = new OrderController(mockService.Object);

        var result = controller.GetYears();

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task UpdateOrder_ShouldReturnOk()
    {
        var mockService = new Mock<IOrderService>();

        mockService.Setup(s => s.UpdateOrderAsync(It.IsAny<UpdateOrderDto>()))
            .Returns(Task.CompletedTask);

        var controller = new OrderController(mockService.Object);

        var dto = new UpdateOrderDto
        {
            OrderId = 1,
            ShipperId = 1,
            ShippingAddress = "Test",
            City = "City",
            Region = "Region",
            Country = "Country",
            PostalCode = "0000",
            Freight = 10,
            Status = 1,

            Products = new List<UpdateOrderDetailDto>()
        };

        var result = await controller.UpdateOrder(1, dto);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task UpdateOrder_ShouldReturnBadRequest_WhenIdMismatch()
    {
        var controller = new OrderController(Mock.Of<IOrderService>());

        var dto = new UpdateOrderDto
        {
            OrderId = 2,
            ShipperId = 1,
            ShippingAddress = "Test",
            City = "City",
            Region = "Region",
            Country = "Country",
            PostalCode = "0000",
            Freight = 10,
            Status = 1,

            Products = new List<UpdateOrderDetailDto>()
        };

        var result = await controller.UpdateOrder(1, dto);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task DeleteOrder_ShouldReturnOk()
    {
        var mockService = new Mock<IOrderService>();

        mockService.Setup(s => s.DeleteOrderAsync(1))
            .ReturnsAsync(true);

        var controller = new OrderController(mockService.Object);

        var result = await controller.DeleteOrder(1);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task UpdateStatus_ShouldReturnOk()
    {
        var mockService = new Mock<IOrderService>();

        mockService.Setup(s => s.UpdateOrderStatusAsync(1, 2))
            .Returns(Task.CompletedTask);

        var controller = new OrderController(mockService.Object);

        var dto = new UpdateOrderStatusDto { Status = 2 };

        var result = await controller.UpdateStatus(1, dto);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task CreateOrder_ShouldReturnOk()
    {
        var mockService = new Mock<IOrderService>();

        mockService.Setup(s => s.CreateOrderAsync(It.IsAny<CreateOrderDto>()))
            .Returns(Task.CompletedTask);

        var controller = new OrderController(mockService.Object);

        var dto = new CreateOrderDto
        {
            CustomerId = "ALFKI",
            EmployeeId = 1,
            ShipperId = 1,
            ShipName = "Test",
            ShippingAddress = "Street 123",
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
                    Quantity = 1,
                    Discount = 0
                }
            }
        };

        var result = await controller.CreateOrder(dto);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public void GetCountriesByYear_ShouldReturnOk()
    {
        var mockService = new Mock<IOrderService>();

        mockService.Setup(s => s.GetOrderYearsByCountryAndYear(2025))
            .Returns(new List<OrdersByCountryDto>
            {
                new OrdersByCountryDto
                {
                    Country = "El Salvador",
                    Total = 5
                }
            });

        var controller = new OrderController(mockService.Object);

        var result = controller.GetCountriesByYear(2025);

        var ok = Assert.IsType<OkObjectResult>(result);

        Assert.NotNull(ok.Value);
    }

    [Fact]
    public void GetCountriesByYear_ShouldReturnBadRequest_WhenException()
    {
        var mockService = new Mock<IOrderService>();

        mockService.Setup(s => s.GetOrderYearsByCountryAndYear(2025))
            .Throws(new Exception("DB Error"));

        var controller = new OrderController(mockService.Object);

        var result = controller.GetCountriesByYear(2025);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void GetMonthlyOrders_ShouldReturnOk()
    {
        var mockService = new Mock<IOrderService>();

        mockService.Setup(s => s.GetOrdersByMonth(2025))
            .Returns(new List<OrdersByMonthDto>
            {
                new OrdersByMonthDto
                {
                    Month = 1,
                    Total = 10
                }
            });

        var controller = new OrderController(mockService.Object);

        var result = controller.GetMonthlyOrders(2025);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public void GetMonthlyOrders_ShouldReturnBadRequest_WhenException()
    {
        var mockService = new Mock<IOrderService>();

        mockService.Setup(s => s.GetOrdersByMonth(2025))
            .Throws(new Exception("DB Error"));

        var controller = new OrderController(mockService.Object);

        var result = controller.GetMonthlyOrders(2025);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task UpdateOrder_ShouldReturnBadRequest_WhenServiceThrows()
    {
        var mockService = new Mock<IOrderService>();

        mockService.Setup(s => s.UpdateOrderAsync(It.IsAny<UpdateOrderDto>()))
            .ThrowsAsync(new Exception("Update failed"));

        var controller = new OrderController(mockService.Object);

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

        var result = await controller.UpdateOrder(1, dto);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task DeleteOrder_ShouldReturnBadRequest_WhenServiceThrows()
    {
        var mockService = new Mock<IOrderService>();

        mockService.Setup(s => s.DeleteOrderAsync(1))
            .ThrowsAsync(new Exception("Delete failed"));

        var controller = new OrderController(mockService.Object);

        var result = await controller.DeleteOrder(1);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task UpdateStatus_ShouldReturnBadRequest_WhenServiceThrows()
    {
        var mockService = new Mock<IOrderService>();

        mockService.Setup(s => s.UpdateOrderStatusAsync(1, 2))
            .ThrowsAsync(new Exception("Invalid transition"));

        var controller = new OrderController(mockService.Object);

        var dto = new UpdateOrderStatusDto
        {
            Status = 2
        };

        var result = await controller.UpdateStatus(1, dto);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task CreateOrder_ShouldReturnBadRequest_WhenServiceThrows()
    {
        var mockService = new Mock<IOrderService>();

        mockService.Setup(s => s.CreateOrderAsync(It.IsAny<CreateOrderDto>()))
            .ThrowsAsync(new Exception("Create failed"));

        var controller = new OrderController(mockService.Object);

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

            Products = new List<UpdateOrderDetailDto>()
        };

        var result = await controller.CreateOrder(dto);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void GetOrders_ShouldCallServiceOnce()
    {
        var mockService = new Mock<IOrderService>();

        mockService.Setup(s => s.GetOrders(null, null, null, null, null))
            .Returns(new List<OrderListDto>());

        var controller = new OrderController(mockService.Object);

        controller.GetOrders(null, null, null, null, null);

        mockService.Verify(
            s => s.GetOrders(null, null, null, null, null),
            Times.Once
        );
    }

    [Fact]
    public void GetOrderDetail_ShouldCallServiceOnce()
    {
        var mockService = new Mock<IOrderService>();

        mockService.Setup(s => s.GetOrderDetail(1))
            .Returns(new OrderDetailDto
            {
                OrderId = 1
            });

        var controller = new OrderController(mockService.Object);

        controller.GetOrderDetail(1);

        mockService.Verify(s => s.GetOrderDetail(1), Times.Once);
    }

}
