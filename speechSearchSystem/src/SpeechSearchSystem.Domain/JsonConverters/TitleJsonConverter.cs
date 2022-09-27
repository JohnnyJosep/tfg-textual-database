using System.Text.Json;

using Newtonsoft.Json;

using SpeechSearchSystem.Domain.ValueObjects;


namespace SpeechSearchSystem.Domain.JsonConverters;

public class TitleJsonConverter : System.Text.Json.Serialization.JsonConverter<Title>
{
    public override Title? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.TokenType == JsonTokenType.Null 
            ? null :
            Title.FromString(reader.GetString()!);
    }

    public override void Write(Utf8JsonWriter writer, Title? value, JsonSerializerOptions options)
    {
        if (value is null) writer.WriteNullValue();
        else writer.WriteStringValue(value.Value);
    }
}


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
