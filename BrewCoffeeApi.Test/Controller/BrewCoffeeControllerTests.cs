using System;
using System.Threading.Tasks;
using CoffeeProject.Controllers;
using CoffeeProject.Model;
using CoffeeProject.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

public class BrewCoffeeControllerTests
{
    private readonly Mock<IBrewCoffeeRepository> _mockBrewRepo;
    private readonly Mock<IWeatherRepository> _mockWeatherRepo;

    public BrewCoffeeControllerTests()
    {
        _mockBrewRepo = new Mock<IBrewCoffeeRepository>();
        _mockWeatherRepo = new Mock<IWeatherRepository>();
    }

    [Fact]
    public async Task Get_ReturnsOk_WhenNormalDayAndNotHot()
    {
        _mockBrewRepo.Setup(r => r.Get()).ReturnsAsync(new BrewCoffeeModel
        {
            Message = "Test Coffee",
            Prepared = new DateTime(2026, 1, 6)
        });

        _mockWeatherRepo.Setup(w => w.GetCurrentConditions(It.IsAny<string>()))
            .ReturnsAsync(new CurrentConditions
            {
                Temperature = new Temperature
                {
                    Metric = new Units { Value = "25", Unit = "C", UnitType = 17 },
                    Imperial = new Units { Value = "77", Unit = "F", UnitType = 18 }
                }
            });

        var controller = new BrewCoffeeController(_mockBrewRepo.Object, _mockWeatherRepo.Object);

        var result = await controller.Get();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<BrewCoffeeResponse>(okResult.Value);
        Assert.Equal("Test Coffee", response.Message);
    }

    [Fact]
    public async Task Get_ReturnsTeapot_OnAprilFools()
    {
        _mockBrewRepo.Setup(r => r.Get()).ReturnsAsync(new BrewCoffeeModel
        {
            Message = "Test Coffee",
            Prepared = new DateTime(2026, 4, 1)
        });

        var controller = new BrewCoffeeController(_mockBrewRepo.Object, _mockWeatherRepo.Object);

        var result = await controller.Get();

        var teapotResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(418, teapotResult.StatusCode);
        Assert.Equal("I'm a teapot", teapotResult.Value);
    }

    [Fact]
    public async Task Get_ReturnsServiceUnavailable_OnFifthCall()
    {

        // Reset call count before test
        BrewCoffeeController.ResetCallCount();


        _mockBrewRepo.Setup(r => r.Get()).ReturnsAsync(new BrewCoffeeModel
        {
            Message = "Test Coffee",
            Prepared = new DateTime(2026, 1, 6)
        });

        _mockWeatherRepo.Setup(w => w.GetCurrentConditions(It.IsAny<string>()))
            .ReturnsAsync(new CurrentConditions
            {
                Temperature = new Temperature
                {
                    Metric = new Units { Value = "25", Unit = "C", UnitType = 17 },
                    Imperial = new Units { Value = "77", Unit = "F", UnitType = 18 }
                }
            });

        var controller = new BrewCoffeeController(_mockBrewRepo.Object, _mockWeatherRepo.Object);

        // Call 4 times
        for (int i = 0; i < 4; i++)
        {
            await controller.Get();
        }

        // 5th call triggers 503
        var result = await controller.Get();

        var serviceResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(503, serviceResult.StatusCode);
        Assert.Equal("Service Unavailable", serviceResult.Value);
    }

    [Fact]
    public async Task Get_AddsIcedCoffeeMessage_WhenHotWeather()
    {
        _mockBrewRepo.Setup(r => r.Get()).ReturnsAsync(new BrewCoffeeModel
        {
            Message = "Test Coffee",
            Prepared = new DateTime(2026, 1, 6)
        });

        _mockWeatherRepo.Setup(w => w.GetCurrentConditions(It.IsAny<string>()))
            .ReturnsAsync(new CurrentConditions
            {
                Temperature = new Temperature
                {
                    Metric = new Units { Value = "35", Unit = "C", UnitType = 17 },
                    Imperial = new Units { Value = "95", Unit = "F", UnitType = 18 }
                }
            });

        var controller = new BrewCoffeeController(_mockBrewRepo.Object, _mockWeatherRepo.Object);

        var result = await controller.Get();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<BrewCoffeeResponse>(okResult.Value);
        Assert.Contains("Your refreshing iced coffee is ready", response.Message);
    }

    [Fact]
    public async Task Get_ReturnsInternalServerError_OnRepositoryException()
    {
        _mockBrewRepo.Setup(r => r.Get()).ThrowsAsync(new Exception("Repository failed"));

        var controller = new BrewCoffeeController(_mockBrewRepo.Object, _mockWeatherRepo.Object);

        var result = await controller.Get();

        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, objectResult.StatusCode);
        Assert.Contains("Something went wrong: Repository failed", objectResult.Value.ToString());
    }
}
