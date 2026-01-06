using CoffeeProject.Model;
using CoffeeProject.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeProject.Controllers
{
    [ApiController]
    [Route("brew-coffee")]
    public class BrewCoffeeController : ControllerBase
    {
        private readonly IBrewCoffeeRepository _brewCoffeeRepository;
        private readonly IWeatherRepository _weatherRepository;
        private static int callCount = 0;


        public BrewCoffeeController(IBrewCoffeeRepository brewCoffeeRepository, IWeatherRepository weatherRepository )
        {
            _brewCoffeeRepository = brewCoffeeRepository;
             _weatherRepository = weatherRepository;
        }


        private void IncrementCallCount() {
            callCount++;
        }


        [HttpGet]
        public async Task<IActionResult> Get()
        {
            IncrementCallCount();
            BrewCoffeeResponse response = new BrewCoffeeResponse();
            try
            {
                var model = await _brewCoffeeRepository.Get();
                response.Message = model.Message;
                response.Prepared = model.Prepared;


                

                
                if (model.IsAprilFoolsDay) {
                    return StatusCode(418, "I'm a teapot");
                }

                

                if (callCount == 5) {
                    callCount = 0;
                    return StatusCode(503, "Service Unavailable");
                }


                var weather = await _weatherRepository.GetCurrentConditions("264885");
                if(Double.Parse(weather.Temperature.Metric.Value) > 30.0) {
                    response.Message += "Your refreshing iced coffee is ready";
                }

                return Ok(response);
               
            }
            catch(Exception ex) {
                return StatusCode(500, $"Something went wrong: {ex.Message}");
            }

            
            
        }

        //for testing purposes
        internal static void ResetCallCount() => callCount = 0;
    }
}
