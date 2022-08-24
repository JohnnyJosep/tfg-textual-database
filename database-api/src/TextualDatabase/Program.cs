using TextualDatabase;
using TextualDatabase.Application;
using TextualDatabase.Application.Handlers.Commands;
using TextualDatabase.Domain.Entities;
using TextualDatabase.Infrastructure;
using TextualDatabase.Queries;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddApplication()
    .AddInfrastructure();

var app = builder.Build();

app.UseSwagger().UseSwaggerUI().UseHttpsRedirection();

app.MapMediateGet<WeatherForecastQuery, WeatherForecast[]>("/weatherforecast/{days}").WithName("GetWeatherForecast");
app.MapMediatePost<AnalizeCommand, string>("/analize").WithName("Analize");

app.Run();
