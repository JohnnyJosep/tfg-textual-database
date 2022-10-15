using System.Globalization;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using SpeechSearchSystem.Domain.Entities;
using SpeechSearchSystem.Domain.Enums;
using SpeechSearchSystem.Domain.ValueObjects;

namespace SpeechSearchSystem.Domain.JsonConverters;

public class SpeechJsonConverter : JsonConverter<Speech>
{
    public override bool CanWrite => false;

    public override void WriteJson(JsonWriter writer, Speech? value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    public override Speech? ReadJson(JsonReader reader, Type objectType, Speech? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null) return null;

        var jo = JObject.Load(reader);

        var title = (string)jo.GetValue("title", StringComparison.InvariantCultureIgnoreCase)!;
        var text = (string)jo.GetValue("text", StringComparison.InvariantCultureIgnoreCase)!;

        var celebratedAtDate = (string)jo.GetValue("celebratedAt", StringComparison.InvariantCultureIgnoreCase)!;

        var source = jo.GetValue("source", StringComparison.InvariantCultureIgnoreCase) as JObject;
        var sourceType = (string)source!.GetValue("type", StringComparison.InvariantCultureIgnoreCase)!;
        var sourceLegislature = (int)source!.GetValue("legislature", StringComparison.InvariantCultureIgnoreCase)!;
        var sourceSession = (int)source!.GetValue("session", StringComparison.InvariantCultureIgnoreCase)!;
        var sourceOrder = (int)source!.GetValue("order", StringComparison.InvariantCultureIgnoreCase)!;

        var author = jo.GetValue("author", StringComparison.InvariantCultureIgnoreCase) as JObject;
        var authorName = (string)author!.GetValue("name", StringComparison.InvariantCultureIgnoreCase)!;
        var authorSurname = (string)author!.GetValue("surname", StringComparison.InvariantCultureIgnoreCase)!;
        var authorGroup = (string)author!.GetValue("group", StringComparison.InvariantCultureIgnoreCase)!;
        var authorFormation = (string)author!.GetValue("formation", StringComparison.InvariantCultureIgnoreCase)!;
        var authorGender = (string)author!.GetValue("gender", StringComparison.InvariantCultureIgnoreCase)!;
        
        var totalInterruptions = (int)jo.GetValue("totalInterruptions", StringComparison.InvariantCultureIgnoreCase)!;
        var interruptionsArray = (jo.GetValue("interruptions", StringComparison.InvariantCultureIgnoreCase) as JArray)?.ToObject<string[]>() ?? Array.Empty<string>();

        Speech speech = new(
            (Title)title,
            (Text)text,
            DateOnly.ParseExact(celebratedAtDate, "yyyy-MM-dd", CultureInfo.InvariantCulture), 
            Source.CreateNew(sourceType, sourceLegislature, sourceSession, sourceOrder),
            Author.CreateNew(authorName, authorSurname, authorGroup, authorFormation, Enum.Parse<Gender>(authorGender)),
            totalInterruptions, 
            interruptionsArray.Select(Interruption.FromString).ToArray()
            );
        return speech;
    }
}