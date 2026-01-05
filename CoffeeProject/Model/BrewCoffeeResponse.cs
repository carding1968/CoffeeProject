using Microsoft.Extensions.Hosting;

namespace CoffeeProject.Model
{
    public class BrewCoffeeResponse
    {
        public string? Message { get; set; }
        public DateTime Prepared { get; set; }

       
    }
}
