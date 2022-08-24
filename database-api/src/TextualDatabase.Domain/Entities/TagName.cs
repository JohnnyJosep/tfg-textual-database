using Ardalis.GuardClauses;

namespace TextualDatabase.Domain.Entities;

public class TagName
{
    public string Name { get; }

    public TagName(string name)
    {
        Name = Guard.Against.NullOrWhiteSpace(name, nameof(name));
    }
}
