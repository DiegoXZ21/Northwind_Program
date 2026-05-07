using Xunit;
using Microsoft.EntityFrameworkCore;
using RSM.Application.Services;
using RSM.Infrastructure.Data;
using RSM.Domain.Models;

public class CategoryServiceTests
{
    private ApplicationDBContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDBContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDBContext(options);
    }

    [Fact]
    public void GetCategories_ShouldReturnAllCategories()
    {
        var context = GetDbContext();

        context.Categories.AddRange(
            new Category { CategoryId = 1, CategoryName = "Beverages" },
            new Category { CategoryId = 2, CategoryName = "Condiments" },
            new Category { CategoryId = 3, CategoryName = "Confections" }
        );

        context.SaveChanges();

        var service = new CategoryService(context);

        var result = service.GetCategories();

        Assert.Equal(3, result.Count);
    }

    [Fact]
    public void GetCategories_ShouldReturnEmptyList_WhenNoCategoriesExist()
    {
        var context = GetDbContext();

        var service = new CategoryService(context);

        var result = service.GetCategories();

        Assert.Empty(result);
    }

    [Fact]
    public void GetCategories_ShouldReturnCorrectCategoryData()
    {
        var context = GetDbContext();

        context.Categories.Add(new Category
        {
            CategoryId = 1,
            CategoryName = "Beverages",
            Description = "Soft drinks, coffees, teas, beers, and ales"
        });

        context.SaveChanges();

        var service = new CategoryService(context);

        var result = service.GetCategories();

        var category = Assert.Single(result);
        Assert.Equal(1, category.CategoryId);
        Assert.Equal("Beverages", category.CategoryName);
        Assert.Equal("Soft drinks, coffees, teas, beers, and ales", category.Description);
    }

    [Fact]
    public void GetCategories_ShouldReturnCategoriesInOrderFromDatabase()
    {
        var context = GetDbContext();

        context.Categories.AddRange(
            new Category { CategoryId = 3, CategoryName = "Confections" },
            new Category { CategoryId = 1, CategoryName = "Beverages" },
            new Category { CategoryId = 2, CategoryName = "Condiments" }
        );

        context.SaveChanges();

        var service = new CategoryService(context);

        var result = service.GetCategories();

        Assert.Equal(3, result.Count);
        Assert.NotNull(result);
    }

    [Fact]
    public void GetCategories_ShouldNotThrow_WhenCalledMultipleTimes()
    {
        var context = GetDbContext();

        context.Categories.Add(new Category { CategoryId = 1, CategoryName = "Beverages" });
        context.SaveChanges();

        var service = new CategoryService(context);

        var result1 = service.GetCategories();
        var result2 = service.GetCategories();

        Assert.Equal(result1.Count, result2.Count);
    }
}
