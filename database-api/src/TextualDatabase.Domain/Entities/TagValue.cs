using Ardalis.GuardClauses;

namespace TextualDatabase.Domain.Entities;

public class TagValue
{
    public string Value { get; }

    public TagValue(string value)
    {
        Value = Guard.Against.NullOrEmpty(value, nameof(value));
    }
}
