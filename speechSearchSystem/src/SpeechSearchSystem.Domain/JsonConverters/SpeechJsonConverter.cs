using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using SpeechSearchSystem.Domain.Entities;
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
        var authorEntry = (string)author!.GetValue("entryDate", StringComparison.InvariantCultureIgnoreCase)!;
        var authorEntryDate = DateOnly.ParseExact(authorEntry, "yyyy-MM-dd");
        var authorLeaving = (string)author!.GetValue("leavingDate", StringComparison.InvariantCultureIgnoreCase)!;
        var authorLeavingDate = string.IsNullOrEmpty(authorLeaving) ? (DateOnly?)null : DateOnly.ParseExact(authorLeaving, "yyyy-MM-dd");

        Speech speech = new Speech(
            (Title)title,
            (Text)text,
            Source.CreateNew(sourceType, sourceLegislature, sourceSession, sourceOrder),
            Author.CreateNew(authorName, authorSurname, authorGroup, authorFormation, authorEntryDate,
                authorLeavingDate));
        return speech;
    }
}