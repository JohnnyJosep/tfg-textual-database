using SpeechSearchSystem.Domain.ValueObjects;
using SpeechSearchSystem.Domain.ValueObjects.Base;

namespace SpeechSearchSystem.Domain.Entities;

public class MorphologicalAnalysis : ValueObject
{
    private readonly List<MorphologicalWordAnalysis> _partOfSpeeches = new();
    public IReadOnlyCollection<MorphologicalWordAnalysis> PartOfSpeech => _partOfSpeeches;


    private MorphologicalAnalysis(string[] rawPoS)
    {
        foreach (string raw in rawPoS)
        {
            _partOfSpeeches.Add(MorphologicalWordAnalysis.Create(raw));
        }
    }

    public static MorphologicalAnalysis Create(string raw) => new(raw.Split(Environment.NewLine));

    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return PartOfSpeech;
    }

    public override string ToString() => string.Join(Environment.NewLine, _partOfSpeeches);
}