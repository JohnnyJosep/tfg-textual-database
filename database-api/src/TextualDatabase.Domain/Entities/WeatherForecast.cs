using System.Text.Json.Serialization;

namespace TextualDatabase.Domain.Entities;

[JsonConverter(typeof(JsonConverters.WeatherForecastJsonConvert))]
public class WeatherForecast
{
    public DateTime Date { get; }
    public int TemperatureC { get; }
    public string? Summary { get; }

    private int? _temperatureF;

    [JsonIgnore]
    public int TemperatureF 
    { 
        get
        {
            if (_temperatureF is null)
            {
                _temperatureF = 32 + (int)(TemperatureC / 0.5556);
            }
            return _temperatureF.Value;
        }
    }


    public WeatherForecast(DateTime date, int temperature, string? summary)
    {
        Date = date;
        TemperatureC = temperature;
        Summary = summary;
    }
}
