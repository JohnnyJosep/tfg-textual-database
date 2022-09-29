using SpeechSearchSystem.Domain.ValueObjects;
using SpeechSearchSystem.Domain.ValueObjects.Base;

namespace SpeechSearchSystem.Domain.Entities;

public class MorphologicalAnalysis : ValueObject
{
    private readonly List<MorphologicalWordAnalysis> _partOfSpeeches = new();
    public IReadOnlyCollection<MorphologicalWordAnalysis> PartOfSpeech => _partOfSpeeches;
    public IReadOnlyCollection<string> Lexemes => _partOfSpeeches.Select(p => p.Lexeme).ToArray();
    public IReadOnlyCollection<string> TaggedLexemes => _partOfSpeeches.Select(p => $"{p.TagSet[0]}:{p.Lexeme}").ToArray();


    private MorphologicalAnalysis(string[] rawPoS)
    {
        foreach (string raw in rawPoS.Where(r => !string.IsNullOrEmpty(r)))
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