using SpeechSearchSystem.Domain.Entities;
using SpeechSearchSystem.Domain.Enums;
using SpeechSearchSystem.Domain.Events.Base;
using SpeechSearchSystem.Domain.ValueObjects;

namespace SpeechSearchSystem.Domain.Events;

public class SpeechCreated : IEvent
{
    private readonly string _id;
    private readonly Title _title;
    private readonly Text _text;
    private readonly Source _source;
    private readonly Author _author;

    private SpeechCreated(string id, Title title, Text text, Source source, Author author)
    {
        _id = id;
        _title = title;
        _text = text;
        _source = source;
        _author = author;
    }

    public static SpeechCreated Create(string id,  Title title, Text text, Source source, Author author) =>
        new(id, title, text, source, author);
}