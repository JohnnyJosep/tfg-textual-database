using System.Text.Json.Serialization;

using SpeechSearchSystem.Domain.JsonConverters;
using SpeechSearchSystem.Domain.ValueObjects;

namespace SpeechSearchSystem.Domain.Entities;

[Newtonsoft.Json.JsonConverter(typeof(SpeechJsonConverter))]
public class Speech
{
    [JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    public string? Id { get; private set; }
    public Title Title { get; }
    public Text Text { get; }
    public Source Source { get; }
    public Author Author { get; }
    public MorphologicalAnalysis? MorphologicalAnalysis { get; private set; }

    public Speech(Title title, Text text, Source source, Author author)
    {
        Title = title;
        Text = text;
        Source = source;
        Author = author;
        MorphologicalAnalysis = null;
    }
    
    public void AddMorphologicalAnalysis(string raw)
    {
        Ensure.That<DomainException>(!string.IsNullOrEmpty(raw), "Morphological analysis raw can not be null or empty.");
        MorphologicalAnalysis = MorphologicalAnalysis.Create(raw);
    }
}