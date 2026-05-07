using Xunit;
using Microsoft.EntityFrameworkCore;
using RSM.Application.Services;
using RSM.Infrastructure.Data;
using RSM.Domain.Models;
using RSM.Application.Dtos;

public class ProductServiceTests
{
    private ApplicationDBContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDBContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDBContext(options);
    }

    #region GetProducts Tests

    [Fact]
    public void GetProducts_ShouldReturnAllProducts_WhenNoFilters()
    {
        var context = GetDbContext();

        var category = new Category { CategoryId = 1, CategoryName = "Beverages" };
        context.Categories.Add(category);

        context.Products.AddRange(
            new Product
            {
                ProductId = 1,
                ProductName = "Chai",
                Category = category,
                UnitPrice = 18,
                UnitsInStock = 39
            },
            new Product
            {
                ProductId = 2,
                ProductName = "Chang",
                Category = category,
                UnitPrice = 19,
                UnitsInStock = 17
            }
        );

        context.SaveChanges();

        var service = new ProductService(context);

        var result = service.GetProducts(null, null);

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void GetProducts_ShouldFilterByName()
    {
        var context = GetDbContext();

        var category = new Category { CategoryId = 1, CategoryName = "Beverages" };
        context.Categories.Add(category);

        context.Products.AddRange(
            new Product
            {
                ProductId = 1,
                ProductName = "Chai",
                Category = category,
                UnitPrice = 18
            },
            new Product
            {
                ProductId = 2,
                ProductName = "Chang",
                Category = category,
                UnitPrice = 19
            }
        );

        context.SaveChanges();

        var service = new ProductService(context);

        var result = service.GetProducts("Chai", null);

        Assert.Single(result);
        Assert.Equal("Chai", result[0].ProductName);
    }

    [Fact]
    public void GetProducts_ShouldFilterByCategory()
    {
        var context = GetDbContext();

        var category1 = new Category { CategoryId = 1, CategoryName = "Beverages" };
        var category2 = new Category { CategoryId = 2, CategoryName = "Condiments" };
        context.Categories.AddRange(category1, category2);

        context.Products.AddRange(
            new Product { ProductId = 1, ProductName = "Chai", Category = category1 },
            new Product { ProductId = 2, ProductName = "Sauce", Category = category2 }
        );

        context.SaveChanges();

        var service = new ProductService(context);

        var result = service.GetProducts(null, "Beverages");

        Assert.Single(result);
        Assert.Equal("Chai", result[0].ProductName);
    }

    [Fact]
    public void GetProducts_ShouldReturnEmptyList_WhenNoMatch()
    {
        var context = GetDbContext();

        var service = new ProductService(context);

        var result = service.GetProducts("NonExistent", null);

        Assert.Empty(result);
    }

    #endregion

    #region GetInventory Tests

    [Fact]
    public void GetInventory_ShouldReturnProductInventoryDto()
    {
        var context = GetDbContext();

        var category = new Category { CategoryId = 1, CategoryName = "Beverages" };
        context.Categories.Add(category);

        context.Products.Add(new Product
        {
            ProductId = 1,
            ProductName = "Chai",
            Category = category,
            UnitPrice = 18,
            UnitsInStock = 39
        });

        context.SaveChanges();

        var service = new ProductService(context);

        var result = service.GetInventory(null, null);

        Assert.Single(result);
        Assert.IsType<ProductInventoryDto>(result[0]);
        Assert.Equal("Chai", result[0].ProductName);
        Assert.Equal((short)39, result[0].UnitsInStock);
        Assert.Equal("Beverages", result[0].CategoryName);
    }

    [Fact]
    public void GetInventory_ShouldFilterByName()
    {
        var context = GetDbContext();

        var category = new Category { CategoryId = 1, CategoryName = "Beverages" };
        context.Categories.Add(category);

        context.Products.AddRange(
            new Product { ProductId = 1, ProductName = "Chai", Category = category, UnitsInStock = 39 },
            new Product { ProductId = 2, ProductName = "Coffee", Category = category, UnitsInStock = 50 }
        );

        context.SaveChanges();

        var service = new ProductService(context);

        var result = service.GetInventory("Chai", null);

        Assert.Single(result);
        Assert.Equal("Chai", result[0].ProductName);
    }

    [Fact]
    public void GetInventory_ShouldFilterByCategory()
    {
        var context = GetDbContext();

        var category = new Category { CategoryId = 1, CategoryName = "Beverages" };
        context.Categories.Add(category);

        context.Products.Add(new Product
        {
            ProductId = 1,
            ProductName = "Chai",
            Category = category,
            UnitsInStock = 39
        });

        context.SaveChanges();

        var service = new ProductService(context);

        var result = service.GetInventory(null, "Beverages");

        Assert.Single(result);
    }

    #endregion

    #region UpdateInventory Tests

    [Fact]
    public void UpdateInventory_ShouldUpdateProductUnitsInStock()
    {
        var context = GetDbContext();

        var category = new Category { CategoryId = 1, CategoryName = "Beverages" };
        context.Categories.Add(category);

        context.Products.Add(new Product
        {
            ProductId = 1,
            ProductName = "Chai",
            Category = category,
            UnitsInStock = 39
        });

        context.SaveChanges();

        var service = new ProductService(context);

        service.UpdateInventory(1, 50);

        var updatedProduct = context.Products.First(p => p.ProductId == 1);
        Assert.Equal((short)50, updatedProduct.UnitsInStock);
    }

    [Fact]
    public void UpdateInventory_ShouldThrow_WhenProductNotFound()
    {
        var context = GetDbContext();

        var service = new ProductService(context);

        Assert.Throws<Exception>(() => service.UpdateInventory(999, 50));
    }

    [Fact]
    public void UpdateInventory_ShouldUpdateToZero()
    {
        var context = GetDbContext();

        var category = new Category
        {
            CategoryId = 1,
            CategoryName = "Beverages"
        };

        context.Categories.Add(category);

        context.Products.Add(new Product
        {
            ProductId = 1,
            ProductName = "Chai",
            Category = category,
            UnitsInStock = 39
        });

        context.SaveChanges();

        var service = new ProductService(context);

        service.UpdateInventory(1, (short)0);

        var updatedProduct = context.Products.First(p => p.ProductId == 1);

        Assert.Equal((short)0, updatedProduct.UnitsInStock);
    }

    #endregion

    #region GetAvailableProducts Tests

    [Fact]
    public void GetAvailableProducts_ShouldReturnOnlyAvailableProducts()
    {
        var context = GetDbContext();

        var category = new Category { CategoryId = 1, CategoryName = "Beverages" };
        context.Categories.Add(category);

        context.Products.AddRange(
            new Product
            {
                ProductId = 1,
                ProductName = "Chai",
                Category = category,
                UnitPrice = 18,
                UnitsInStock = 39,
                Discontinued = false
            },
            new Product
            {
                ProductId = 2,
                ProductName = "Discontinued Product",
                Category = category,
                UnitPrice = 10,
                UnitsInStock = 5,
                Discontinued = true
            },
            new Product
            {
                ProductId = 3,
                ProductName = "Out of Stock",
                Category = category,
                UnitPrice = 10,
                UnitsInStock = 0,
                Discontinued = false
            }
        );

        context.SaveChanges();

        var service = new ProductService(context);

        var result = service.GetAvailableProducts();

        Assert.Single(result);
        Assert.Equal("Chai", result[0].ProductName);
    }

    [Fact]
    public void GetAvailableProducts_ShouldReturnCorrectDto()
    {
        var context = GetDbContext();

        var category = new Category { CategoryId = 1, CategoryName = "Beverages" };
        context.Categories.Add(category);

        context.Products.Add(new Product
        {
            ProductId = 1,
            ProductName = "Chai",
            Category = category,
            UnitPrice = 18,
            UnitsInStock = 39,
            QuantityPerUnit = "10 boxes x 20 bags",
            Discontinued = false
        });

        context.SaveChanges();

        var service = new ProductService(context);

        var result = service.GetAvailableProducts();

        var dto = Assert.Single(result);
        Assert.IsType<AvailableProductDto>(dto);
        Assert.Equal(1, dto.ProductId);
        Assert.Equal("Chai", dto.ProductName);
        Assert.Equal(18, dto.UnitPrice);
        Assert.Equal(39, dto.UnitsInStock);
        Assert.Equal("10 boxes x 20 bags", dto.QuantityPerUnit);
    }

    [Fact]
    public void GetAvailableProducts_ShouldReturnEmptyList_WhenNoAvailableProducts()
    {
        var context = GetDbContext();

        var service = new ProductService(context);

        var result = service.GetAvailableProducts();

        Assert.Empty(result);
    }

    [Fact]
    public void GetAvailableProducts_ShouldHandleNullUnitPrice()
    {
        var context = GetDbContext();

        var category = new Category { CategoryId = 1, CategoryName = "Beverages" };
        context.Categories.Add(category);

        context.Products.Add(new Product
        {
            ProductId = 1,
            ProductName = "Chai",
            Category = category,
            UnitPrice = null,
            UnitsInStock = 10,
            Discontinued = false
        });

        context.SaveChanges();

        var service = new ProductService(context);

        var result = service.GetAvailableProducts();

        Assert.Single(result);
        Assert.Equal(0, result[0].UnitPrice);
    }

    [Fact]
    public void GetAvailableProducts_ShouldHandleNullUnitsInStock()
    {
        var context = GetDbContext();

        var category = new Category { CategoryId = 1, CategoryName = "Beverages" };
        context.Categories.Add(category);

        context.Products.Add(new Product
        {
            ProductId = 1,
            ProductName = "Chai",
            Category = category,
            UnitPrice = 18,
            UnitsInStock = null,
            Discontinued = false
        });

        context.SaveChanges();

        var service = new ProductService(context);

        var result = service.GetAvailableProducts();

        Assert.Empty(result);
    }

    #endregion
}
