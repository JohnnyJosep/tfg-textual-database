using SpeechSearchSystem.Domain.ValueObjects.Base;

namespace SpeechSearchSystem.Domain.ValueObjects;

public class Author : ValueObject
{
    public string Name { get; }
    public string Surname { get; }
    public string Group { get; }
    public string Formation { get; }
    public DateOnly EntryDate { get; }
    public DateOnly? LeavingDate { get; }

    private Author(string name, string surname, string group, string formation, DateOnly entryDate, DateOnly? leavingDate)
    {
        Ensure.That<DomainException>(!string.IsNullOrWhiteSpace(name), "Name can not be null or white space.");
        Ensure.That<DomainException>(!string.IsNullOrWhiteSpace(surname), "Surname can not be null or white space.");
        Ensure.That<DomainException>(!string.IsNullOrWhiteSpace(group), "Group can not be null or white space.");
        Ensure.That<DomainException>(!string.IsNullOrWhiteSpace(formation), "Formation can not be null or white space.");

        Name = name;
        Surname = surname;
        Group = group;
        Formation = formation;
        EntryDate = entryDate;
        LeavingDate = leavingDate;
    }

    public static Author CreateNew(
        string name, string surname, string group, string formation, DateOnly entryDate, DateOnly? leavingDate) =>
        new(name, surname, group, formation, entryDate, leavingDate);

    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return Name;
        yield return Surname;
        yield return Group;
        yield return Formation;
        yield return EntryDate;
        yield return LeavingDate;
    }

    public override string ToString() => $"{Name} {Surname} ({Formation})";
}