using CoffeeProject.Model;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CoffeeProject
{
    public class ApiCounterFilter : IActionFilter
    {
        private readonly ApiRequestCounter _counter;

        public ApiCounterFilter(ApiRequestCounter counter)
        {
            _counter = counter;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            Interlocked.Increment(ref _counter.TotalCount);
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}
