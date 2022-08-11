using MediatR;

using TextualDatabase.Handlers.Base;

namespace TextualDatabase.Queries;

internal record WheatherForecastQuery(int Days) : IHttpRequest;

internal class WheatherForecastHandler : IRequestHandler<WheatherForecastQuery, IResult>
{
    public async Task<IResult> Handle(WheatherForecastQuery request, CancellationToken cancellationToken)
    {
        await Task.Delay(1, cancellationToken);

        var summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        var forecast = Enumerable.Range(1, request.Days).Select(index => new WeatherForecast(
                DateTime.Now.AddDays(index),
                Random.Shared.Next(-20, 55),
                summaries[Random.Shared.Next(summaries.Length)]))
            .ToArray();
        return Results.Ok(forecast);
    }
}

internal record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}