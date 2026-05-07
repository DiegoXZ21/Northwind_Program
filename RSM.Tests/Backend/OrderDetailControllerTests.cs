using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using RSM.Backend.Controllers;
using RSM.Application.Services;
using RSM.Application.Dtos;

public class OrderDetailControllerTests
{
    [Fact]
    public void GetOrderDetail_ShouldReturnOkResult()
    {
        var mockService = new Mock<IOrderDetailService>();

        mockService.Setup(s => s.GetOrderDetail(1))
            .Returns(new OrderWithProductsDto { OrderId = 1 });

        var controller = new OrderDetailController(mockService.Object);

        var result = controller.GetOrderDetail(1);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public void GetOrderDetail_ShouldReturnOrderWithProductsDto()
    {
        var mockService = new Mock<IOrderDetailService>();

        var dto = new OrderWithProductsDto
        {
            OrderId = 1,
            CustomerName = "Alfreds Futterkiste",
            ShippingAddress = "123 Main St",
            City = "Berlin",
            Country = "Germany",
            Freight = 32.38m,
            Status = 1,
            Products = new List<OrderProductDto>()
        };

        mockService.Setup(s => s.GetOrderDetail(1))
            .Returns(dto);

        var controller = new OrderDetailController(mockService.Object);

        var result = controller.GetOrderDetail(1);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedDto = Assert.IsType<OrderWithProductsDto>(okResult.Value);

        Assert.Equal(1, returnedDto.OrderId);
        Assert.Equal("Alfreds Futterkiste", returnedDto.CustomerName);
    }

    [Fact]
    public void GetOrderDetail_ShouldReturnBadRequest_WhenOrderNotFound()
    {
        var mockService = new Mock<IOrderDetailService>();

        mockService.Setup(s => s.GetOrderDetail(999))
            .Throws(new Exception("Order not found"));

        var controller = new OrderDetailController(mockService.Object);

        var result = controller.GetOrderDetail(999);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void GetOrderDetail_ShouldReturnBadRequest_WhenException()
    {
        var mockService = new Mock<IOrderDetailService>();

        mockService.Setup(s => s.GetOrderDetail(It.IsAny<int>()))
            .Throws(new Exception("Database error"));

        var controller = new OrderDetailController(mockService.Object);

        var result = controller.GetOrderDetail(1);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void GetOrderDetail_ShouldIncludeOrderProducts()
    {
        var mockService = new Mock<IOrderDetailService>();

        var dto = new OrderWithProductsDto
        {
            OrderId = 1,
            CustomerName = "Alfreds",
            Products = new List<OrderProductDto>
            {
                new OrderProductDto
                {
                    ProductId = 1,
                    ProductName = "Chai",
                    UnitPrice = 18,
                    Quantity = 10,
                    Discount = 0
                },
                new OrderProductDto
                {
                    ProductId = 2,
                    ProductName = "Chang",
                    UnitPrice = 19,
                    Quantity = 5,
                    Discount = 0
                }
            }
        };

        mockService.Setup(s => s.GetOrderDetail(1))
            .Returns(dto);

        var controller = new OrderDetailController(mockService.Object);

        var result = controller.GetOrderDetail(1);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedDto = Assert.IsType<OrderWithProductsDto>(okResult.Value);

        Assert.Equal(2, returnedDto.Products.Count);
    }

    [Fact]
    public void GetOrderDetail_ShouldCallServiceWithCorrectOrderId()
    {
        var mockService = new Mock<IOrderDetailService>();

        mockService.Setup(s => s.GetOrderDetail(42))
            .Returns(new OrderWithProductsDto { OrderId = 42 });

        var controller = new OrderDetailController(mockService.Object);

        controller.GetOrderDetail(42);

        mockService.Verify(s => s.GetOrderDetail(42), Times.Once);
    }

    [Fact]
    public void GetOrderDetail_ShouldReturnCompleteOrderDetails()
    {
        var mockService = new Mock<IOrderDetailService>();

        var dto = new OrderWithProductsDto
        {
            OrderId = 1,
            CustomerName = "Test Customer",
            OrderDate = new DateTime(2025, 1, 15),
            ShippingAddress = "123 Test St",
            City = "Test City",
            Region = "Test Region",
            Country = "Test Country",
            PostalCode = "12345",
            ShipperId = 1,
            Status = 1,
            Freight = 10.50m,
            Latitude = 52.52m,
            Longitude = 13.40m,
            Products = new List<OrderProductDto>()
        };

        mockService.Setup(s => s.GetOrderDetail(1))
            .Returns(dto);

        var controller = new OrderDetailController(mockService.Object);

        var result = controller.GetOrderDetail(1);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedDto = Assert.IsType<OrderWithProductsDto>(okResult.Value);

        Assert.Equal(1, returnedDto.OrderId);
        Assert.Equal("Test Customer", returnedDto.CustomerName);
        Assert.Equal("123 Test St", returnedDto.ShippingAddress);
        Assert.Equal(1, returnedDto.ShipperId);
    }

    [Fact]
    public void GetOrderDetail_ShouldHandleZeroFreight()
    {
        var mockService = new Mock<IOrderDetailService>();

        var dto = new OrderWithProductsDto
        {
            OrderId = 1,
            CustomerName = "Test",
            Freight = 0,
            Products = new List<OrderProductDto>()
        };

        mockService.Setup(s => s.GetOrderDetail(1))
            .Returns(dto);

        var controller = new OrderDetailController(mockService.Object);

        var result = controller.GetOrderDetail(1);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedDto = Assert.IsType<OrderWithProductsDto>(okResult.Value);

        Assert.Equal(0, returnedDto.Freight);
    }
}
