using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using RSM.Backend.Controllers;
using RSM.Application.Services;
using RSM.Application.Dtos;
using RSM.Domain.Models;

public class ProductControllerTests
{
    [Fact]
    public void GetProducts_ShouldReturnOkResult()
    {
        var mockService = new Mock<IProductService>();

        mockService.Setup(s => s.GetProducts(null, null))
            .Returns(new List<Product>());

        var controller = new ProductController(mockService.Object);

        var result = controller.GetProducts(null, null);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public void GetProducts_ShouldReturnProducts()
    {
        var mockService = new Mock<IProductService>();

        var products = new List<Product>
        {
            new Product { ProductId = 1, ProductName = "Chai", UnitPrice = 18 },
            new Product { ProductId = 2, ProductName = "Chang", UnitPrice = 19 }
        };

        mockService.Setup(s => s.GetProducts(null, null))
            .Returns(products);

        var controller = new ProductController(mockService.Object);

        var result = controller.GetProducts(null, null);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedProducts = Assert.IsType<List<Product>>(okResult.Value);

        Assert.Equal(2, returnedProducts.Count);
    }

    [Fact]
    public void GetProducts_ShouldFilterByName()
    {
        var mockService = new Mock<IProductService>();

        mockService.Setup(s => s.GetProducts("Chai", null))
            .Returns(new List<Product>
            {
                new Product { ProductId = 1, ProductName = "Chai", UnitPrice = 18 }
            });

        var controller = new ProductController(mockService.Object);

        var result = controller.GetProducts("Chai", null);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedProducts = Assert.IsType<List<Product>>(okResult.Value);

        Assert.Single(returnedProducts);
    }

    [Fact]
    public void GetProducts_ShouldReturnBadRequest_WhenException()
    {
        var mockService = new Mock<IProductService>();

        mockService.Setup(s => s.GetProducts(null, null))
            .Throws(new Exception("Database error"));

        var controller = new ProductController(mockService.Object);

        var result = controller.GetProducts(null, null);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void GetInventory_ShouldReturnOkResult()
    {
        var mockService = new Mock<IProductService>();

        mockService.Setup(s => s.GetInventory(null, null))
            .Returns(new List<ProductInventoryDto>());

        var controller = new ProductController(mockService.Object);

        var result = controller.GetInventory(null, null);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public void GetInventory_ShouldReturnProductInventoryDto()
    {
        var mockService = new Mock<IProductService>();

        var inventory = new List<ProductInventoryDto>
        {
            new ProductInventoryDto
            {
                ProductId = 1,
                ProductName = "Chai",
                UnitsInStock = 39,
                CategoryName = "Beverages"
            }
        };

        mockService.Setup(s => s.GetInventory(null, null))
            .Returns(inventory);

        var controller = new ProductController(mockService.Object);

        var result = controller.GetInventory(null, null);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedInventory = Assert.IsType<List<ProductInventoryDto>>(okResult.Value);

        Assert.Single(returnedInventory);
    }

    [Fact]
    public void GetInventory_ShouldReturnBadRequest_WhenException()
    {
        var mockService = new Mock<IProductService>();

        mockService.Setup(s => s.GetInventory(null, null))
            .Throws(new Exception("Database error"));

        var controller = new ProductController(mockService.Object);

        var result = controller.GetInventory(null, null);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateInventory_ShouldReturnNoContent()
    {
        var mockService = new Mock<IProductService>();

        mockService.Setup(s => s.UpdateInventory(It.IsAny<int>(), It.IsAny<short>()))
            .Throws<Exception>();

        var controller = new ProductController(mockService.Object);

        var dto = new UpdateInventoryDto { UnitsInStock = 50 };

        var result = controller.UpdateInventory(1, dto);

        // Should handle exception, so returns BadRequest
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateInventory_ShouldCallService()
    {
        var mockService = new Mock<IProductService>();

        mockService.Setup(s => s.UpdateInventory(1, 50));

        var controller = new ProductController(mockService.Object);

        var dto = new UpdateInventoryDto { UnitsInStock = 50 };

        controller.UpdateInventory(1, dto);

        mockService.Verify(s => s.UpdateInventory(1, 50), Times.Once);
    }

    [Fact]
    public void UpdateInventory_ShouldReturnBadRequest_WhenProductNotFound()
    {
        var mockService = new Mock<IProductService>();

        mockService.Setup(s => s.UpdateInventory(999, It.IsAny<short>()))
            .Throws(new Exception("Product not found"));

        var controller = new ProductController(mockService.Object);

        var dto = new UpdateInventoryDto { UnitsInStock = 50 };

        var result = controller.UpdateInventory(999, dto);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void GetAvailableProducts_ShouldReturnOkResult()
    {
        var mockService = new Mock<IProductService>();

        mockService.Setup(s => s.GetAvailableProducts())
            .Returns(new List<AvailableProductDto>());

        var controller = new ProductController(mockService.Object);

        var result = controller.GetAvailableProducts();

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public void GetAvailableProducts_ShouldReturnAvailableProducts()
    {
        var mockService = new Mock<IProductService>();

        var availableProducts = new List<AvailableProductDto>
        {
            new AvailableProductDto
            {
                ProductId = 1,
                ProductName = "Chai",
                UnitPrice = 18,
                UnitsInStock = 39,
                QuantityPerUnit = "10 boxes x 20 bags"
            }
        };

        mockService.Setup(s => s.GetAvailableProducts())
            .Returns(availableProducts);

        var controller = new ProductController(mockService.Object);

        var result = controller.GetAvailableProducts();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedProducts = Assert.IsType<List<AvailableProductDto>>(okResult.Value);

        Assert.Single(returnedProducts);
    }

    [Fact]
    public void GetAvailableProducts_ShouldReturnBadRequest_WhenException()
    {
        var mockService = new Mock<IProductService>();

        mockService.Setup(s => s.GetAvailableProducts())
            .Throws(new Exception("Database error"));

        var controller = new ProductController(mockService.Object);

        var result = controller.GetAvailableProducts();

        Assert.IsType<BadRequestObjectResult>(result);
    }
}
