using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using RSM.Backend.Controllers;
using RSM.Application.Services;
using RSM.Application.Dtos;

public class EmployeeControllerTests
{
    [Fact]
    public void GetEmployees_ShouldReturnOkResult()
    {
        var mockService = new Mock<IEmployeeService>();

        mockService.Setup(s => s.GetEmployees())
            .Returns(new List<EmployeeDto>
            {
                new EmployeeDto
                {
                    EmployeeId = 1,
                    EmployeeName = "Diego Guerrero"
                }
            });

        var controller = new EmployeeController(mockService.Object);

        var result = controller.GetEmployees();

        var okResult = Assert.IsType<OkObjectResult>(result);

        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public void GetEmployees_ShouldReturnCorrectData()
    {
        var mockService = new Mock<IEmployeeService>();

        var expected = new List<EmployeeDto>
        {
            new EmployeeDto
            {
                EmployeeId = 1,
                EmployeeName = "Ana Lopez"
            }
        };

        mockService.Setup(s => s.GetEmployees())
            .Returns(expected);

        var controller = new EmployeeController(mockService.Object);

        var result = controller.GetEmployees();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var data = Assert.IsType<List<EmployeeDto>>(okResult.Value);

        Assert.Single(data);
        Assert.Equal("Ana Lopez", data[0].EmployeeName);
    }

    [Fact]
    public void GetEmployees_ShouldCallServiceOnce()
    {
        var mockService = new Mock<IEmployeeService>();

        mockService.Setup(s => s.GetEmployees())
            .Returns(new List<EmployeeDto>());

        var controller = new EmployeeController(mockService.Object);

        controller.GetEmployees();

        mockService.Verify(s => s.GetEmployees(), Times.Once);
    }
}