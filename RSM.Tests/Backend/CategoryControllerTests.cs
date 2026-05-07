using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using RSM.Backend.Controllers;
using RSM.Application.Services;
using RSM.Domain.Models;

public class CategoryControllerTests
{
    [Fact]
    public void GetCategories_ShouldReturnOkResult()
    {
        var mockService = new Mock<ICategoryService>();

        mockService.Setup(s => s.GetCategories())
            .Returns(new List<Category>
            {
                new Category { CategoryId = 1, CategoryName = "Beverages" }
            });

        var controller = new CategoryController(mockService.Object);

        var result = controller.GetCategories();

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public void GetCategories_ShouldReturnAllCategories()
    {
        var mockService = new Mock<ICategoryService>();

        var categories = new List<Category>
        {
            new Category { CategoryId = 1, CategoryName = "Beverages", Description = "Soft drinks, coffees, teas, beers, and ales" },
            new Category { CategoryId = 2, CategoryName = "Condiments", Description = "Sweet and savory sauces, spreads, and seasonings" },
            new Category { CategoryId = 3, CategoryName = "Confections", Description = "Desserts, candies, and sweet breads" }
        };

        mockService.Setup(s => s.GetCategories())
            .Returns(categories);

        var controller = new CategoryController(mockService.Object);

        var result = controller.GetCategories();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedCategories = Assert.IsType<List<Category>>(okResult.Value);

        Assert.Equal(3, returnedCategories.Count);
    }

    [Fact]
    public void GetCategories_ShouldReturnEmptyList_WhenNoCategoriesExist()
    {
        var mockService = new Mock<ICategoryService>();

        mockService.Setup(s => s.GetCategories())
            .Returns(new List<Category>());

        var controller = new CategoryController(mockService.Object);

        var result = controller.GetCategories();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedCategories = Assert.IsType<List<Category>>(okResult.Value);

        Assert.Empty(returnedCategories);
    }

    [Fact]
    public void GetCategories_ShouldReturnBadRequest_WhenException()
    {
        var mockService = new Mock<ICategoryService>();

        mockService.Setup(s => s.GetCategories())
            .Throws(new Exception("Database error"));

        var controller = new CategoryController(mockService.Object);

        var result = controller.GetCategories();

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void GetCategories_ShouldCallServiceOnce()
    {
        var mockService = new Mock<ICategoryService>();

        mockService.Setup(s => s.GetCategories())
            .Returns(new List<Category>());

        var controller = new CategoryController(mockService.Object);

        controller.GetCategories();

        mockService.Verify(s => s.GetCategories(), Times.Once);
    }

    [Fact]
    public void GetCategories_ShouldReturnCorrectCategoryData()
    {
        var mockService = new Mock<ICategoryService>();

        var categories = new List<Category>
        {
            new Category { CategoryId = 1, CategoryName = "Beverages", Description = "Drinks" }
        };

        mockService.Setup(s => s.GetCategories())
            .Returns(categories);

        var controller = new CategoryController(mockService.Object);

        var result = controller.GetCategories();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedCategories = Assert.IsType<List<Category>>(okResult.Value);

        var category = Assert.Single(returnedCategories);
        Assert.Equal(1, category.CategoryId);
        Assert.Equal("Beverages", category.CategoryName);
    }

    [Fact]
    public void GetCategories_ShouldReturnBadRequest_WhenInvalidOperation()
    {
        var mockService = new Mock<ICategoryService>();

        mockService.Setup(s => s.GetCategories())
            .Throws(new InvalidOperationException("Invalid operation"));

        var controller = new CategoryController(mockService.Object);

        var result = controller.GetCategories();

        var badResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.NotNull(badResult.Value);
    }
}
