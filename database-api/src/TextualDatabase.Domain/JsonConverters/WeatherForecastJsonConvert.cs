using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

using TextualDatabase.Domain.Entities;

namespace TextualDatabase.Domain.JsonConverters;

internal class WeatherForecastJsonConvert : JsonConverter<WeatherForecast>
{
    public override WeatherForecast Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException();
        reader.Read();

        if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException();
        if (reader.GetString() != "date") throw new JsonException();

        reader.Read();
        var dateString = reader.GetString();
        if (string.IsNullOrEmpty(dateString) || 
            !DateTime.TryParseExact(dateString, "O", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date)) 
            throw new JsonException();

        reader.Read();
        if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException();
        if (reader.GetString() != "summary") throw new JsonException();
        reader.Read();
        var summaryValue = reader.GetString();
        if (string.IsNullOrEmpty(summaryValue)) throw new JsonException();
        var summary = summaryValue;

        reader.Read();
        if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException();
        if (reader.GetString() != "celsius") throw new JsonException();
        reader.Read();
        var celsius = reader.GetInt32();

        reader.Read();
        if (reader.TokenType != JsonTokenType.EndObject) throw new JsonException();

        return new WeatherForecast(date, celsius, summary);
    }

    public override void Write(Utf8JsonWriter writer, WeatherForecast value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteString("date", value.Date.ToString("O"));
        writer.WriteString("summary", value.Summary);
        writer.WriteNumber("celsius", value.TemperatureC);
        writer.WriteEndObject();
    }
}
