using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using RSM.Backend.Controllers;
using RSM.Application.Services;
using RSM.Application.Dtos;
using RSM.Application.Services.External;

public class GoogleAddressControllerTests
{
    [Fact]
    public async Task Validate_ShouldReturnOk_WithResult()
    {
        var mockService = new Mock<IGoogleAddressService>();

        var expected = new AddressValidationResult
        {
            IsValid = true,
            Latitude = 13.7,
            Longitude = -89.2,
            FormattedAddress = "San Salvador"
        };

        mockService
            .Setup(s => s.ValidateAddressAsync(It.IsAny<AddressDto>()))
            .ReturnsAsync(expected);

        var controller = new GoogleAddressController(mockService.Object);

        var dto = new AddressDto
        {
            Address = "Street",
            City = "City",
            Region = "Region",
            PostalCode = "0000"
        };

        var result = await controller.Validate(dto);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<AddressValidationResult>(okResult.Value);

        Assert.True(value.IsValid);
    }

    [Fact]
    public async Task Validate_ShouldCallServiceOnce()
    {
        var mockService = new Mock<IGoogleAddressService>();

        mockService
            .Setup(s => s.ValidateAddressAsync(It.IsAny<AddressDto>()))
            .ReturnsAsync(new AddressValidationResult());

        var controller = new GoogleAddressController(mockService.Object);

        await controller.Validate(new AddressDto());

        mockService.Verify(s => s.ValidateAddressAsync(It.IsAny<AddressDto>()), Times.Once);
    }

    [Fact]
    public async Task Validate_ShouldReturnResult_EvenIfInvalid()
    {
        var mockService = new Mock<IGoogleAddressService>();

        var expected = new AddressValidationResult
        {
            IsValid = false
        };

        mockService
            .Setup(s => s.ValidateAddressAsync(It.IsAny<AddressDto>()))
            .ReturnsAsync(expected);

        var controller = new GoogleAddressController(mockService.Object);

        var result = await controller.Validate(new AddressDto());

        var okResult = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<AddressValidationResult>(okResult.Value);

        Assert.False(value.IsValid);
    }
}