using CoffeeProject;
using CoffeeProject.Model;
using CoffeeProject.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IBrewCoffeeRepository, BrewCoffeeRepository>();
builder.Services.AddScoped<IWeatherRepository, AccuWeatherRepository>();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(option => {
        option.SwaggerEndpoint("/openapi/v1.json", "Coffee Project API");
    });
    app.MapSwagger();
    app.UseSwaggerUI();
}

/*if (app.Environment.IsProduction())
{
    app.MapOpenApi();
    app.UseSwaggerUI(option => {
        option.SwaggerEndpoint("/openapi/v1.json", "WebSolution API");
    });
    app.MapSwagger();
    app.UseSwaggerUI();
}*/



app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
