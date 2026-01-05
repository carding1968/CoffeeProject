using CoffeeProject.Model;

namespace CoffeeProject.Repositories
{
    public interface IBrewCoffeeRepository
    {
        Task<BrewCoffeeModel> Get();
    }
}
