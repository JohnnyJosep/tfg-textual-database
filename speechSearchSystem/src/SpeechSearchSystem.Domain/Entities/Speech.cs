using SpeechSearchSystem.Domain.JsonConverters;
using SpeechSearchSystem.Domain.ValueObjects;

namespace SpeechSearchSystem.Domain.Entities;

[Newtonsoft.Json.JsonConverter(typeof(SpeechJsonConverter))]
public class Speech
{
    public Title Title { get; }
    public Text Text { get; }
    public DateOnly CelebratedAt { get; }
    public Source Source { get; }
    public Author Author { get; }
    public int TotalInterruptions { get; }
    public IReadOnlyCollection<Interruption> Interruptions { get; }
    public MorphologicalAnalysis? MorphologicalAnalysis { get; private set; }

    public Speech(Title title, Text text, DateOnly celebratedAt, Source source, Author author, int totalInterruptions, IReadOnlyCollection<Interruption> interruptions)
    {
        Ensure.That<DomainException>(totalInterruptions == interruptions.Count, "TotalInterruptions and Interruptions count must be the same");

        Title = title;
        Text = text;
        CelebratedAt = celebratedAt;
        Source = source;
        Author = author;
        TotalInterruptions = totalInterruptions;
        Interruptions = interruptions;
        MorphologicalAnalysis = null;
    }
    
    public void AddMorphologicalAnalysis(string raw)
    {
        Ensure.That<DomainException>(!string.IsNullOrEmpty(raw), "Morphological analysis raw can not be null or empty.");
        MorphologicalAnalysis = MorphologicalAnalysis.Create(raw);
    }
}