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
        private readonly Mock<IBrewCoffeeRepository> _mockRepo;

        public BrewCoffeeControllerTests()
        {
            _mockRepo = new Mock<IBrewCoffeeRepository>();
        }

        [Fact]
        public async Task Get_ReturnsOk_WhenNormalDay()
        {
            // Arrange
            _mockRepo.Setup(r => r.Get()).ReturnsAsync(new BrewCoffeeModel
            {
                Message = "Test Coffee",
                Prepared = new DateTime(2026, 1, 6) // normal day
            });

            var controller = new BrewCoffeeController(_mockRepo.Object);

            // Act
            var result = await controller.Get();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<BrewCoffeeResponse>(okResult.Value);
            Assert.Equal("Test Coffee", response.Message);
            Assert.True(true);
        }

        [Fact]
        public async Task Get_ReturnsTeapot_OnAprilFools()
        {
            // Arrange
            _mockRepo.Setup(r => r.Get()).ReturnsAsync(new BrewCoffeeModel
            {
                Message = "Test Coffee",
                Prepared = new DateTime(2026, 4, 1) // April 1
            });

            var controller = new BrewCoffeeController(_mockRepo.Object);

            // Act
            var result = await controller.Get();

            // Assert
            var teapotResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(418, teapotResult.StatusCode);
            Assert.Equal("I'm a teapot", teapotResult.Value);
        }

        [Fact]
        public async Task Get_ReturnsServiceUnavailable_OnFifthCall()
        {
            // Reset call count before test
            BrewCoffeeController.ResetCallCount();

            // Arrange
            _mockRepo.Setup(r => r.Get()).ReturnsAsync(new BrewCoffeeModel
            {
                Message = "Test Coffee",
                Prepared = new DateTime(2026, 1, 6)
            });

            var controller = new BrewCoffeeController(_mockRepo.Object);

            // Call 4 times first
            for (int i = 0; i < 4; i++)
            {
                await controller.Get();
            }

            // Act - 5th call
            var result = await controller.Get();

            // Assert
            var serviceResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(503, serviceResult.StatusCode);
            Assert.Equal("Service Unavailable", serviceResult.Value);
        }


        [Fact]
        public async Task Get_ReturnsInternalServerError_OnException()
        {
            // Arrange
            _mockRepo.Setup(r => r.Get()).ThrowsAsync(new Exception("Repository failed"));

            var controller = new BrewCoffeeController(_mockRepo.Object);

            // Act
            var result = await controller.Get();

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
            Assert.Contains("Something went wrong: Repository failed", objectResult.Value.ToString());
        }
    }

}
