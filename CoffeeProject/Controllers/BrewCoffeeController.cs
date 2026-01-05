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

        public BrewCoffeeController(IBrewCoffeeRepository brewCoffeeRepository, ApiRequestCounter counter)
        {
            _brewCoffeeRepository = brewCoffeeRepository;
            _counter = counter;
        }


        [HttpGet(Name = "GetBrewCoffee")]
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

                var test = _counter.TotalCount;

                if (_counter.TotalCount == 5) {
                    _counter.TotalCount = 0;
                    return StatusCode(503, "Service Unavailable");
                }

                return Ok(response);
               
            }
            catch(Exception ex) {
                return StatusCode(500, $"Something went wrong: {ex.Message}");
            }

            
            
        }
    }
}
