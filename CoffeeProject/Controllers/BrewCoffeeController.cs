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
        private ApiRequestCounter _counter;
        private readonly IWeatherRepository _weatherRepository;

        public BrewCoffeeController(IBrewCoffeeRepository brewCoffeeRepository, ApiRequestCounter counter, IWeatherRepository weatherRepository )
        {
            _brewCoffeeRepository = brewCoffeeRepository;
            _counter = counter;
            _weatherRepository = weatherRepository;
        }


        [HttpGet]
        [ServiceFilter(typeof(ApiCounterFilter))]
        public async Task<IActionResult> Get()
        {
            BrewCoffeeResponse response = new BrewCoffeeResponse();
            try
            {
                var model = await _brewCoffeeRepository.Get();
                response.Message = model.Message;
                response.Prepared = model.Prepared;


                

                
                if (model.IsAprilFoolsDay) {
                    return StatusCode(418, "I'm a teapot");
                }

                

                if (_counter.TotalCount == 5) {
                    _counter.TotalCount = 0;
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
    }
}
