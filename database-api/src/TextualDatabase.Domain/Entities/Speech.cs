using Ardalis.GuardClauses;

using TextualDatabase.Domain.Enums;

namespace TextualDatabase.Domain.Entities;

public class Speech
{
    public string RawText { get; }
    public IDictionary<TagName, TagValue> Tags { get; }
    public SpeechStatus Status { get; }

    private Speech(string rawText, IDictionary<TagName, TagValue> tags)
    {
        RawText = Guard.Against.NullOrEmpty(rawText, nameof(rawText));
        Tags = tags;

        Status = SpeechStatus.Indexing;
    }

    public static Speech Create(string rawText, IDictionary<TagName, TagValue> tags)
    {
        var speech = new Speech(rawText, tags);

        return speech;
    }
}
