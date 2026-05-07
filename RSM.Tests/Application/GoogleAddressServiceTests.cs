using Xunit;
using Microsoft.EntityFrameworkCore;
using Moq;
using RSM.Application.Services;
using RSM.Application.Dtos;
using RSM.Infrastructure.Data;
using RSM.Domain.Models;
using RSM.Application.Services.External;
using Moq.Protected;
using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;

public class GoogleAddressServiceTests
{
    [Fact]
    public async Task ValidateAddress_ShouldReturnValidResult_WhenApiReturnsSuccess()
    {
        var handlerMock = new Mock<HttpMessageHandler>();

        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Content = JsonContent.Create(new
                {
                    result = new
                    {
                        geocode = new
                        {
                            location = new
                            {
                                latitude = 13.7,
                                longitude = -89.2
                            }
                        },
                        address = new
                        {
                            formattedAddress = "San Salvador"
                        },
                        verdict = new { }
                    }
                })
            });

        var httpClient = new HttpClient(handlerMock.Object);

        var configMock = new Mock<IConfiguration>();
        configMock.Setup(c => c["Google:ApiKey"]).Returns("test-key");

        var service = new GoogleAddressService(httpClient, configMock.Object);

        var dto = new AddressDto
        {
            Address = "Street",
            City = "City",
            Region = "Region",
            PostalCode = "0000"
        };

        var result = await service.ValidateAddressAsync(dto);

        Assert.True(result.IsValid);
        Assert.Equal(13.7, result.Latitude);
        Assert.Equal(-89.2, result.Longitude);
    }

    [Fact]
    public async Task ValidateAddress_ShouldReturnInvalid_WhenApiFails()
    {
        var handlerMock = new Mock<HttpMessageHandler>();

        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = System.Net.HttpStatusCode.BadRequest
            });

        var httpClient = new HttpClient(handlerMock.Object);

        var configMock = new Mock<IConfiguration>();
        configMock.Setup(c => c["Google:ApiKey"]).Returns("test-key");

        var service = new GoogleAddressService(httpClient, configMock.Object);

        var result = await service.ValidateAddressAsync(new AddressDto());

        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task ValidateAddress_ShouldReturnInvalid_WhenLocationIsNull()
    {
        var handlerMock = new Mock<HttpMessageHandler>();

        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Content = JsonContent.Create(new
                {
                    result = new
                    {
                        geocode = new
                        {
                            location = (object?)null
                        }
                    }
                })
            });

        var httpClient = new HttpClient(handlerMock.Object);

        var configMock = new Mock<IConfiguration>();
        configMock.Setup(c => c["Google:ApiKey"]).Returns("test-key");

        var service = new GoogleAddressService(httpClient, configMock.Object);

        var result = await service.ValidateAddressAsync(new AddressDto());

        Assert.False(result.IsValid);
    }
}