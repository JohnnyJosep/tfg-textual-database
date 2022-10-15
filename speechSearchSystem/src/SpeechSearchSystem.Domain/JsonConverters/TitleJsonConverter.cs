using Newtonsoft.Json;

using SpeechSearchSystem.Domain.ValueObjects;

namespace SpeechSearchSystem.Domain.JsonConverters;

public class TitleNewtonsoftJsonConverter : Newtonsoft.Json.JsonConverter<Title>
{
    public override Title? ReadJson(JsonReader reader, Type objectType, Title? existingValue, bool hasExistingValue,
        Newtonsoft.Json.JsonSerializer serializer) =>
        reader.TokenType == JsonToken.Null
            ? null
            : Title.FromString((string)reader.Value!);


    public override void WriteJson(JsonWriter writer, Title? value, Newtonsoft.Json.JsonSerializer serializer)
    {
        if (value is null) writer.WriteNull();
        else writer.WriteValue(value.Value);
    }

}
