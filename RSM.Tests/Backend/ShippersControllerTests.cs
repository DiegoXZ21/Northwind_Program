using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using RSM.Backend.Controllers;
using RSM.Application.Services;
using RSM.Application.Dtos;

public class ShippersControllerTests
{
    [Fact]
    public void GetShippers_ShouldReturnOkResult()
    {
        var mockService = new Mock<IShipperService>();

        mockService.Setup(s => s.GetShippers())
            .Returns(new List<ShipperDto>());

        var controller = new ShippersController(mockService.Object);

        var result = controller.GetShippers();

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public void GetShippers_ShouldReturnAllShippers()
    {
        var mockService = new Mock<IShipperService>();

        var shippers = new List<ShipperDto>
        {
            new ShipperDto { ShipperId = 1, CompanyName = "Speedy Express" },
            new ShipperDto { ShipperId = 2, CompanyName = "United Package" },
            new ShipperDto { ShipperId = 3, CompanyName = "Federal Shipping" }
        };

        mockService.Setup(s => s.GetShippers())
            .Returns(shippers);

        var controller = new ShippersController(mockService.Object);

        var result = controller.GetShippers();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedShippers = Assert.IsType<List<ShipperDto>>(okResult.Value);

        Assert.Equal(3, returnedShippers.Count);
    }

    [Fact]
    public void GetShippers_ShouldReturnShipperDto()
    {
        var mockService = new Mock<IShipperService>();

        var shippers = new List<ShipperDto>
        {
            new ShipperDto { ShipperId = 1, CompanyName = "Speedy Express" }
        };

        mockService.Setup(s => s.GetShippers())
            .Returns(shippers);

        var controller = new ShippersController(mockService.Object);

        var result = controller.GetShippers();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedShippers = Assert.IsType<List<ShipperDto>>(okResult.Value);

        var shipper = Assert.Single(returnedShippers);
        Assert.Equal(1, shipper.ShipperId);
        Assert.Equal("Speedy Express", shipper.CompanyName);
    }

    [Fact]
    public void GetShippers_ShouldReturnEmptyList_WhenNoShippersExist()
    {
        var mockService = new Mock<IShipperService>();

        mockService.Setup(s => s.GetShippers())
            .Returns(new List<ShipperDto>());

        var controller = new ShippersController(mockService.Object);

        var result = controller.GetShippers();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedShippers = Assert.IsType<List<ShipperDto>>(okResult.Value);

        Assert.Empty(returnedShippers);
    }

    [Fact]
    public void GetShippers_ShouldReturnBadRequest_WhenException()
    {
        var mockService = new Mock<IShipperService>();

        mockService.Setup(s => s.GetShippers())
            .Throws(new Exception("Database error"));

        var controller = new ShippersController(mockService.Object);

        var result = controller.GetShippers();

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void GetShippers_ShouldCallServiceOnce()
    {
        var mockService = new Mock<IShipperService>();

        mockService.Setup(s => s.GetShippers())
            .Returns(new List<ShipperDto>());

        var controller = new ShippersController(mockService.Object);

        controller.GetShippers();

        mockService.Verify(s => s.GetShippers(), Times.Once);
    }

    [Fact]
    public void GetShippers_ShouldReturnCorrectData()
    {
        var mockService = new Mock<IShipperService>();

        var expectedShippers = new List<ShipperDto>
        {
            new ShipperDto { ShipperId = 1, CompanyName = "Shipper A" },
            new ShipperDto { ShipperId = 2, CompanyName = "Shipper B" }
        };

        mockService.Setup(s => s.GetShippers())
            .Returns(expectedShippers);

        var controller = new ShippersController(mockService.Object);

        var result = controller.GetShippers();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedShippers = Assert.IsType<List<ShipperDto>>(okResult.Value);

        Assert.Equal("Shipper A", returnedShippers[0].CompanyName);
        Assert.Equal("Shipper B", returnedShippers[1].CompanyName);
    }
}
