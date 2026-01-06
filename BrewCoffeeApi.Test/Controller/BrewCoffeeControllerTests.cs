using CoffeeProject.Controllers;
using CoffeeProject.Model;
using Microsoft.AspNetCore.Mvc;
using CoffeeProject.Repositories;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace BrewCoffeeApi.Tests.Controller
{
    
    
    public class BrewCoffeeControllerTests
    {
        [Fact]
        public async Task Get_ReturnsOk_When_NormalRequest()
        {
            // Arrange
            var repoMock1 = new Mock<IBrewCoffeeRepository>();
            var repoMoc2 = new Mock<IWeatherRepository>();
            repoMock1.Setup(r => r.Get()).ReturnsAsync(new BrewCoffeeModel
            {
                Message = "Coffee Ready",
                Prepared = DateTime.Now
                
            });

            var controller = new BrewCoffeeController(repoMock1.Object, repoMoc2.Object);

            // Act
            var result = await controller.Get();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<BrewCoffeeResponse>(okResult.Value);
            Assert.Equal("Coffee Ready", response.Message);
            Assert.True(true);
        }



        [Fact]
        public async Task Get_Returns503_And_ResetsCounter_When_TotalCountEquals5()
        {
            // Arrange
            var repoMock1 = new Mock<IBrewCoffeeRepository>();
            var repoMoc2 = new Mock<IWeatherRepository>();
            repoMock1.Setup(r => r.Get()).ReturnsAsync(new BrewCoffeeModel
            {
                Message = "Coffee Ready",
                Prepared = DateTime.Now,
                
            });

            var controller = new BrewCoffeeController(repoMock1.Object, repoMoc2.Object);

            // Act
            var result = await controller.Get();

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(503, objectResult.StatusCode);
            Assert.Equal("Service Unavailable", objectResult.Value);

            // Counter should be reset
            Assert.Equal(0, 5);
        }

        [Fact]
        public async Task Get_Returns500_When_RepositoryThrowsException()
        {
            // Arrange
            var repoMock1 = new Mock<IBrewCoffeeRepository>();
            var repoMoc2 = new Mock<IWeatherRepository>();
            repoMock1.Setup(r => r.Get()).ThrowsAsync(new Exception("Test exception"));

            var controller = new BrewCoffeeController(repoMock1.Object, repoMoc2.Object);

            // Act
            var result = await controller.Get();

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
            Assert.Contains("Test exception", objectResult.Value.ToString());
        }
    }

}
