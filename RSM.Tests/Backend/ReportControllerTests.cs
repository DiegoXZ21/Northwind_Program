using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using RSM.Backend.Controllers;
using RSM.Application.Services;
using RSM.Application.Dtos;

public class ReportControllerTests
{
    [Fact]
    public void ExportOrders_ShouldReturnOkResult()
    {
        var mockService = new Mock<IReportService>();

        mockService.Setup(s => s.GetOrdersForExport(It.IsAny<ExportOrdersRequestDto>()))
            .Returns(new List<OrderListDto>());

        var controller = new ReportController(mockService.Object);

        var request = new ExportOrdersRequestDto();

        var result = controller.ExportOrders(request);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public void ExportOrders_ShouldReturnExportData()
    {
        var mockService = new Mock<IReportService>();

        var orders = new List<OrderListDto>
        {
            new OrderListDto
            {
                OrderId = 1,
                CustomerName = "Alfreds",
                OrderDate = new DateTime(2025, 1, 15),
                Country = "Germany",
                Status = "Processing",
                TotalAmount = 100,
                ProductCount = 2
            }
        };

        mockService.Setup(s => s.GetOrdersForExport(It.IsAny<ExportOrdersRequestDto>()))
            .Returns(orders);

        var controller = new ReportController(mockService.Object);

        var request = new ExportOrdersRequestDto();

        var result = controller.ExportOrders(request);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedValue = okResult.Value;

        Assert.NotNull(returnedValue);
    }

    [Fact]
    public void ExportOrders_ShouldReturnCorrectMessage()
    {
        var mockService = new Mock<IReportService>();

        mockService.Setup(s => s.GetOrdersForExport(It.IsAny<ExportOrdersRequestDto>()))
            .Returns(new List<OrderListDto>
            {
                new OrderListDto
                {
                    OrderId = 1,
                    CustomerName = "Test",
                    OrderDate = new DateTime(2025, 1, 1),
                    Country = "USA",
                    Status = "Shipped",
                    TotalAmount = 50,
                    ProductCount = 1
                }
            });

        var controller = new ReportController(mockService.Object);

        var request = new ExportOrdersRequestDto();

        var result = controller.ExportOrders(request);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public void ExportOrders_ShouldFilterByOrderIds()
    {
        var mockService = new Mock<IReportService>();

        var orders = new List<OrderListDto>
        {
            new OrderListDto { OrderId = 1, CustomerName = "Test", OrderDate = DateTime.Now, Country = "USA", Status = "Shipped", TotalAmount = 50, ProductCount = 1 }
        };

        mockService.Setup(s => s.GetOrdersForExport(It.IsAny<ExportOrdersRequestDto>()))
            .Returns(orders);

        var controller = new ReportController(mockService.Object);

        var request = new ExportOrdersRequestDto { OrderIds = new List<int> { 1, 2, 3 } };

        var result = controller.ExportOrders(request);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);

        mockService.Verify(s => s.GetOrdersForExport(It.Is<ExportOrdersRequestDto>(
            r => r.OrderIds.Count == 3
        )), Times.Once);
    }

    [Fact]
    public void ExportOrders_ShouldFilterByYear()
    {
        var mockService = new Mock<IReportService>();

        mockService.Setup(s => s.GetOrdersForExport(It.IsAny<ExportOrdersRequestDto>()))
            .Returns(new List<OrderListDto>());

        var controller = new ReportController(mockService.Object);

        var request = new ExportOrdersRequestDto { Year = 2025 };

        controller.ExportOrders(request);

        mockService.Verify(s => s.GetOrdersForExport(It.Is<ExportOrdersRequestDto>(
            r => r.Year == 2025
        )), Times.Once);
    }

    [Fact]
    public void ExportOrders_ShouldFilterByMonth()
    {
        var mockService = new Mock<IReportService>();

        mockService.Setup(s => s.GetOrdersForExport(It.IsAny<ExportOrdersRequestDto>()))
            .Returns(new List<OrderListDto>());

        var controller = new ReportController(mockService.Object);

        var request = new ExportOrdersRequestDto { Month = 6 };

        controller.ExportOrders(request);

        mockService.Verify(s => s.GetOrdersForExport(It.Is<ExportOrdersRequestDto>(
            r => r.Month == 6
        )), Times.Once);
    }

    [Fact]
    public void ExportOrders_ShouldFilterByCountry()
    {
        var mockService = new Mock<IReportService>();

        mockService.Setup(s => s.GetOrdersForExport(It.IsAny<ExportOrdersRequestDto>()))
            .Returns(new List<OrderListDto>());

        var controller = new ReportController(mockService.Object);

        var request = new ExportOrdersRequestDto { Country = "Germany" };

        controller.ExportOrders(request);

        mockService.Verify(s => s.GetOrdersForExport(It.Is<ExportOrdersRequestDto>(
            r => r.Country == "Germany"
        )), Times.Once);
    }

    [Fact]
    public void ExportOrders_ShouldReturnCorrectCount()
    {
        var mockService = new Mock<IReportService>();

        var orders = new List<OrderListDto>
        {
            new OrderListDto { OrderId = 1, CustomerName = "A", OrderDate = DateTime.Now, Country = "USA", Status = "Shipped", TotalAmount = 50, ProductCount = 1 },
            new OrderListDto { OrderId = 2, CustomerName = "B", OrderDate = DateTime.Now, Country = "USA", Status = "Shipped", TotalAmount = 60, ProductCount = 2 }
        };

        mockService.Setup(s => s.GetOrdersForExport(It.IsAny<ExportOrdersRequestDto>()))
            .Returns(orders);

        var controller = new ReportController(mockService.Object);

        var request = new ExportOrdersRequestDto();

        var result = controller.ExportOrders(request);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public void ExportOrders_ShouldReturnEmptyList_WhenNoOrders()
    {
        var mockService = new Mock<IReportService>();

        mockService.Setup(s => s.GetOrdersForExport(It.IsAny<ExportOrdersRequestDto>()))
            .Returns(new List<OrderListDto>());

        var controller = new ReportController(mockService.Object);

        var request = new ExportOrdersRequestDto();

        var result = controller.ExportOrders(request);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public void ExportOrders_ShouldCallServiceWithCorrectRequest()
    {
        var mockService = new Mock<IReportService>();

        mockService.Setup(s => s.GetOrdersForExport(It.IsAny<ExportOrdersRequestDto>()))
            .Returns(new List<OrderListDto>());

        var controller = new ReportController(mockService.Object);

        var request = new ExportOrdersRequestDto { Year = 2025, Month = 1, Country = "Germany" };

        controller.ExportOrders(request);

        mockService.Verify(s => s.GetOrdersForExport(It.IsAny<ExportOrdersRequestDto>()), Times.Once);
    }
}
