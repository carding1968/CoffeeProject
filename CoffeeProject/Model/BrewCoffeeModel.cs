namespace CoffeeProject.Model
{
    public class BrewCoffeeModel
    {

        public string? Message { get; set; } = "Your piping hot coffee is ready";
        public DateTime Prepared {  get; set; } = DateTime.Now;

        public bool IsAprilFoolsDay => (Prepared.Month == 4) && (Prepared.Day == 1);
    }
}
