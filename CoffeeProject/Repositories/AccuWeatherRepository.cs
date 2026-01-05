using CoffeeProject.Model;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Buffers.Text;
using System.Security.Principal;

namespace CoffeeProject.Repositories
{
    public class AccuWeatherRepository : IWeatherRepository
    {
        private readonly IConfiguration _configuration;
       
        public AccuWeatherRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public const string AUTO_COMPLETE_ENDPOINT = "/locations/v1/cities/autocomplete?apikey={0}&q={1}";
        public const string CURRENT_CONDITION_ENDPOINT = "/currentconditions/v1/{0}?apikey={1}";

        public async Task<CurrentConditions> GetCurrentConditions(string cityKey)
        {
            CurrentConditions currentConditions = new CurrentConditions();

            string url = _configuration.GetSection("AccuWeather")["Baseurl"] + string.Format(CURRENT_CONDITION_ENDPOINT, cityKey, _configuration.GetSection("AccuWeather")["ApiKey"]);

            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(url);
                string json = await response.Content.ReadAsStringAsync();

                currentConditions = (JsonConvert.DeserializeObject<List<CurrentConditions>>(json)).FirstOrDefault();
            }


            return currentConditions;
        }
    }
}
