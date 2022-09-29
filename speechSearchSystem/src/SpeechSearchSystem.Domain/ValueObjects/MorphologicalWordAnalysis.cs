using SpeechSearchSystem.Domain.ValueObjects.Base;

namespace SpeechSearchSystem.Domain.ValueObjects;

public class MorphologicalWordAnalysis : ValueObject
{
    public string Word { get; }
    public string Lexeme { get; }
    public string TagSet { get; }
    public double Probability { get; }

    private MorphologicalWordAnalysis(string word, string lexeme, string tagSet, double probability)
    {
        Ensure.That<DomainException>(!string.IsNullOrWhiteSpace(word), "Word can not be null or empty");
        Ensure.That<DomainException>(!string.IsNullOrWhiteSpace(lexeme), "Lexeme can not be null or empty");
        Ensure.That<DomainException>(!string.IsNullOrWhiteSpace(tagSet), "TagSet can not be null or empty");
        Ensure.That<DomainException>(probability >= 0, "Probability must be greater or equals to zero");

        Word = word;
        Lexeme = lexeme;
        TagSet = tagSet;
        Probability = probability;
    }

    public static MorphologicalWordAnalysis Create(string raw)
    {
        var parts = raw.Trim().Split(' ');
        return new MorphologicalWordAnalysis(parts[0], parts[1], parts[2], double.Parse(parts[3]));
    }

    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return Word;
        yield return Lexeme;
        yield return TagSet;
        yield return Probability;
    }

    public override string ToString() => $"{Word} {Lexeme} {TagSet} {Probability}";
}