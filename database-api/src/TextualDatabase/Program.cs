using MediatR;

using TextualDatabase;
using TextualDatabase.Handlers.Queries;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddMediatR(x => x.AsScoped(), typeof(Program));

var app = builder.Build();

app.UseSwagger().UseSwaggerUI().UseHttpsRedirection();

app.MapMediateGet<WheaterForecastQuery>("/weatherforecast/{days}").WithName("GetWeatherForecast");

app.Run();
