using System.Text.Json;

using MediatR;

using Microsoft.Extensions.Caching.Distributed;

using TextualDatabase.Domain.Entities;

namespace TextualDatabase.Queries;

public record WeatherForecastQuery(int Days) : IRequest<WeatherForecast[]>;

internal class WeatherForecastHandler : IRequestHandler<WeatherForecastQuery, WeatherForecast[]>
{
    private readonly IDistributedCache _cache;

    public WeatherForecastHandler(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<WeatherForecast[]> Handle(WeatherForecastQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"weather_{request.Days}";

        var json = await _cache.GetStringAsync(cacheKey, cancellationToken);

        if (!string.IsNullOrEmpty(json))
        {
            var cachedForecasts = JsonSerializer.Deserialize<WeatherForecast[]>(json);
            return cachedForecasts!;
        }

        var summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        var forecasts = Enumerable.Range(1, request.Days).Select(index => new WeatherForecast(
                DateTime.Now.AddDays(index),
                Random.Shared.Next(-20, 55),
                summaries[Random.Shared.Next(summaries.Length)]))
            .ToArray();

        var options = new DistributedCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromHours(8));

        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(forecasts), options, cancellationToken);

        return forecasts;

    }
}