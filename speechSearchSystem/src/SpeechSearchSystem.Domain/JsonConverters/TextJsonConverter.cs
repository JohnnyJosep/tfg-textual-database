using System.Text.Json;

using Newtonsoft.Json;

using SpeechSearchSystem.Domain.ValueObjects;

using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace SpeechSearchSystem.Domain.JsonConverters;

public class TextJsonConverter : System.Text.Json.Serialization.JsonConverter<Text>
{
    public override Text? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        reader.TokenType == JsonTokenType.Null 
            ? null 
            : Text.FromString(reader.GetString()!);


    public override void Write(Utf8JsonWriter writer, Text? value, JsonSerializerOptions options)
    {
        if (value is null) writer.WriteNullValue();
        else writer.WriteStringValue(value.Value);
    }

}

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

