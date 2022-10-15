using Newtonsoft.Json;

using SpeechSearchSystem.Domain.ValueObjects;

namespace SpeechSearchSystem.Domain.JsonConverters;

public class TextNewtonsoftJsonConverter : JsonConverter<Text>
{
    public override Text? ReadJson(JsonReader reader, Type objectType, Text? existingValue, bool hasExistingValue,
        JsonSerializer serializer) =>
        reader.TokenType == JsonToken.Null 
            ? null
            : Text.FromString((string)reader.Value!);


    public override void WriteJson(JsonWriter writer, Text? value, JsonSerializer serializer)
    {
        if (value is null) writer.WriteNull();
        else writer.WriteValue(value.Value);
    }

}

