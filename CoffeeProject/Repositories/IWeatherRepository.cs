using CoffeeProject.Model;

namespace CoffeeProject.Repositories
{
    public interface IWeatherRepository
    {
        Task<CurrentConditions> GetCurrentConditions(string cityKey);
    }
}
