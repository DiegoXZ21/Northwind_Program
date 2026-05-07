using Xunit;
using Microsoft.EntityFrameworkCore;
using RSM.Application.Services;
using RSM.Infrastructure.Data;
using RSM.Domain.Models;
using RSM.Application.Dtos;

public class ShipperServiceTests
{
    private ApplicationDBContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDBContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDBContext(options);
    }

    [Fact]
    public void GetShippers_ShouldReturnAllShippers()
    {
        var context = GetDbContext();

        context.Shippers.AddRange(
            new Shipper { ShipperId = 1, CompanyName = "Speedy Express", Phone = "(503) 555-9831" },
            new Shipper { ShipperId = 2, CompanyName = "United Package", Phone = "(503) 555-3199" },
            new Shipper { ShipperId = 3, CompanyName = "Federal Shipping", Phone = "(503) 555-9997" }
        );

        context.SaveChanges();

        var service = new ShipperService(context);

        var result = service.GetShippers();

        Assert.Equal(3, result.Count);
    }

    [Fact]
    public void GetShippers_ShouldReturnShipperDto()
    {
        var context = GetDbContext();

        context.Shippers.Add(new Shipper
        {
            ShipperId = 1,
            CompanyName = "Speedy Express",
            Phone = "(503) 555-9831"
        });

        context.SaveChanges();

        var service = new ShipperService(context);

        var result = service.GetShippers();

        var dto = Assert.Single(result);
        Assert.IsType<ShipperDto>(dto);
        Assert.Equal(1, dto.ShipperId);
        Assert.Equal("Speedy Express", dto.CompanyName);
    }

    [Fact]
    public void GetShippers_ShouldReturnEmptyList_WhenNoShippersExist()
    {
        var context = GetDbContext();

        var service = new ShipperService(context);

        var result = service.GetShippers();

        Assert.Empty(result);
    }

    [Fact]
    public void GetShippers_ShouldMapCorrectCompanyNames()
    {
        var context = GetDbContext();

        context.Shippers.AddRange(
            new Shipper { ShipperId = 1, CompanyName = "Speedy Express" },
            new Shipper { ShipperId = 2, CompanyName = "United Package" }
        );

        context.SaveChanges();

        var service = new ShipperService(context);

        var result = service.GetShippers();

        Assert.Equal("Speedy Express", result[0].CompanyName);
        Assert.Equal("United Package", result[1].CompanyName);
    }

    [Fact]
    public void GetShippers_ShouldMapCorrectShipperIds()
    {
        var context = GetDbContext();

        context.Shippers.Add(new Shipper { ShipperId = 42, CompanyName = "Test Shipper" });

        context.SaveChanges();

        var service = new ShipperService(context);

        var result = service.GetShippers();

        var dto = Assert.Single(result);
        Assert.Equal(42, dto.ShipperId);
    }

    [Fact]
    public void GetShippers_ShouldReturnMultipleShippersCorrectly()
    {
        var context = GetDbContext();

        context.Shippers.AddRange(
            new Shipper { ShipperId = 1, CompanyName = "Shipper A" },
            new Shipper { ShipperId = 2, CompanyName = "Shipper B" },
            new Shipper { ShipperId = 3, CompanyName = "Shipper C" }
        );

        context.SaveChanges();

        var service = new ShipperService(context);

        var result = service.GetShippers();

        Assert.Equal(3, result.Count);
        Assert.All(result, dto => Assert.NotNull(dto.CompanyName));
    }
}
