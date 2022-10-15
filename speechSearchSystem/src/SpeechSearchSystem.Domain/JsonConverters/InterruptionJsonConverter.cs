using Newtonsoft.Json;

using SpeechSearchSystem.Domain.ValueObjects;

namespace SpeechSearchSystem.Domain.JsonConverters;

public class InterruptionJsonConverter : JsonConverter<Interruption>
{
    public override void WriteJson(JsonWriter writer, Interruption? value, JsonSerializer serializer)
    {
        if (value is null) writer.WriteNull();
        else writer.WriteValue(value.Value);
    }

    public override Interruption? ReadJson(JsonReader reader, Type objectType, Interruption? existingValue, bool hasExistingValue,
        JsonSerializer serializer) =>
        reader.TokenType == JsonToken.Null
            ? null
            : Interruption.FromString((string)reader.Value!);
}