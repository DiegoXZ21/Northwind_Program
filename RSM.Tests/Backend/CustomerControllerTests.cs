using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using RSM.Backend.Controllers;
using RSM.Application.Services;
using RSM.Application.Dtos;

public class CustomerControllerTests
{
    [Fact]
    public void GetCustomerDtos_ShouldReturnOkResult()
    {
        var mockService = new Mock<ICustomerService>();

        mockService.Setup(s => s.GetCustomerDtos(null, null, null))
            .Returns(new List<CustomerDto>());

        var controller = new CustomerController(mockService.Object);

        var result = controller.GetCustomerDtos(null, null, null);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public void GetCustomerDtos_ShouldReturnAllCustomers()
    {
        var mockService = new Mock<ICustomerService>();

        var customers = new List<CustomerDto>
        {
            new CustomerDto { CustomerId = "ALFKI", CompanyName = "Alfreds Futterkiste", City = "Berlin", Country = "Germany" },
            new CustomerDto { CustomerId = "ANATR", CompanyName = "Ana Trujillo Emparedados y helados", City = "México D.F.", Country = "Mexico" }
        };

        mockService.Setup(s => s.GetCustomerDtos(null, null, null))
            .Returns(customers);

        var controller = new CustomerController(mockService.Object);

        var result = controller.GetCustomerDtos(null, null, null);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedCustomers = Assert.IsType<List<CustomerDto>>(okResult.Value);

        Assert.Equal(2, returnedCustomers.Count);
    }

    [Fact]
    public void GetCustomerDtos_ShouldFilterByName()
    {
        var mockService = new Mock<ICustomerService>();

        var customers = new List<CustomerDto>
        {
            new CustomerDto { CustomerId = "ALFKI", CompanyName = "Alfreds Futterkiste" }
        };

        mockService.Setup(s => s.GetCustomerDtos("Alfreds", null, null))
            .Returns(customers);

        var controller = new CustomerController(mockService.Object);

        var result = controller.GetCustomerDtos("Alfreds", null, null);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedCustomers = Assert.IsType<List<CustomerDto>>(okResult.Value);

        Assert.Single(returnedCustomers);
    }

    [Fact]
    public void GetCustomerDtos_ShouldFilterByCity()
    {
        var mockService = new Mock<ICustomerService>();

        var customers = new List<CustomerDto>
        {
            new CustomerDto { CustomerId = "ALFKI", CompanyName = "Alfreds", City = "Berlin" }
        };

        mockService.Setup(s => s.GetCustomerDtos(null, "Berlin", null))
            .Returns(customers);

        var controller = new CustomerController(mockService.Object);

        var result = controller.GetCustomerDtos(null, "Berlin", null);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedCustomers = Assert.IsType<List<CustomerDto>>(okResult.Value);

        Assert.Single(returnedCustomers);
    }

    [Fact]
    public void GetCustomerDtos_ShouldFilterByCountry()
    {
        var mockService = new Mock<ICustomerService>();

        var customers = new List<CustomerDto>
        {
            new CustomerDto { CustomerId = "ALFKI", CompanyName = "Alfreds", Country = "Germany" }
        };

        mockService.Setup(s => s.GetCustomerDtos(null, null, "Germany"))
            .Returns(customers);

        var controller = new CustomerController(mockService.Object);

        var result = controller.GetCustomerDtos(null, null, "Germany");

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedCustomers = Assert.IsType<List<CustomerDto>>(okResult.Value);

        Assert.Single(returnedCustomers);
    }

    [Fact]
    public void GetCustomerDtos_ShouldReturnBadRequest_WhenException()
    {
        var mockService = new Mock<ICustomerService>();

        mockService.Setup(s => s.GetCustomerDtos(null, null, null))
            .Throws(new Exception("Database error"));

        var controller = new CustomerController(mockService.Object);

        var result = controller.GetCustomerDtos(null, null, null);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void GetCustomerShippingDtos_ShouldReturnOkResult()
    {
        var mockService = new Mock<ICustomerService>();

        mockService.Setup(s => s.GetCustomerShippingDtos("ALFKI"))
            .Returns(new List<CustomerShippingDto>());

        var controller = new CustomerController(mockService.Object);

        var result = controller.GetCustomerShippingDtos("ALFKI");

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public void GetCustomerShippingDtos_ShouldReturnShippingInfo()
    {
        var mockService = new Mock<ICustomerService>();

        var shippingDtos = new List<CustomerShippingDto>
        {
            new CustomerShippingDto
            {
                Address = "Obere Str. 57",
                City = "Berlin",
                Region = "Berlin",
                PostalCode = "12209",
                Country = "Germany"
            }
        };

        mockService.Setup(s => s.GetCustomerShippingDtos("ALFKI"))
            .Returns(shippingDtos);

        var controller = new CustomerController(mockService.Object);

        var result = controller.GetCustomerShippingDtos("ALFKI");

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedDtos = Assert.IsType<List<CustomerShippingDto>>(okResult.Value);

        Assert.Single(returnedDtos);
        Assert.Equal("Obere Str. 57", returnedDtos[0].Address);
    }

    [Fact]
    public void GetCustomerShippingDtos_ShouldReturnBadRequest_WhenException()
    {
        var mockService = new Mock<ICustomerService>();

        mockService.Setup(s => s.GetCustomerShippingDtos("ALFKI"))
            .Throws(new Exception("Database error"));

        var controller = new CustomerController(mockService.Object);

        var result = controller.GetCustomerShippingDtos("ALFKI");

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void GetCustomerDtos_ShouldReturnEmptyList_WhenNoMatches()
    {
        var mockService = new Mock<ICustomerService>();

        mockService.Setup(s => s.GetCustomerDtos("NonExistent", null, null))
            .Returns(new List<CustomerDto>());

        var controller = new CustomerController(mockService.Object);

        var result = controller.GetCustomerDtos("NonExistent", null, null);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedCustomers = Assert.IsType<List<CustomerDto>>(okResult.Value);

        Assert.Empty(returnedCustomers);
    }

    [Fact]
    public void GetCustomerShippingDtos_ShouldReturnEmptyList_WhenNoMatches()
    {
        var mockService = new Mock<ICustomerService>();

        mockService.Setup(s => s.GetCustomerShippingDtos("INVALID"))
            .Returns(new List<CustomerShippingDto>());

        var controller = new CustomerController(mockService.Object);

        var result = controller.GetCustomerShippingDtos("INVALID");

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedDtos = Assert.IsType<List<CustomerShippingDto>>(okResult.Value);

        Assert.Empty(returnedDtos);
    }

    [Fact]
    public void GetCustomerDtos_ShouldCallServiceWithCorrectParameters()
    {
        var mockService = new Mock<ICustomerService>();

        mockService.Setup(s => s.GetCustomerDtos("Test", "Berlin", "Germany"))
            .Returns(new List<CustomerDto>());

        var controller = new CustomerController(mockService.Object);

        controller.GetCustomerDtos("Test", "Berlin", "Germany");

        mockService.Verify(s => s.GetCustomerDtos("Test", "Berlin", "Germany"), Times.Once);
    }

    [Fact]
    public void GetCustomerShippingDtos_ShouldCallServiceWithCorrectId()
    {
        var mockService = new Mock<ICustomerService>();

        mockService.Setup(s => s.GetCustomerShippingDtos("ALFKI"))
            .Returns(new List<CustomerShippingDto>());

        var controller = new CustomerController(mockService.Object);

        controller.GetCustomerShippingDtos("ALFKI");

        mockService.Verify(s => s.GetCustomerShippingDtos("ALFKI"), Times.Once);
    }
}
