using SpeechSearchSystem.Domain.ValueObjects.Base;

namespace SpeechSearchSystem.Domain.ValueObjects;

public class Source : ValueObject
{
    public string Type { get; }
    public int Legislature { get; }
    public int Session { get; }
    public int Order { get; }

    private Source(string type, int legislature, int session, int order)
    {
        Ensure.That<DomainException>(!string.IsNullOrEmpty(type), "Type can not be null or empty.");
        Ensure.That<DomainException>(legislature > 0, "Legislature must be greater than zero.");
        Ensure.That<DomainException>(session >= 0, "Session must be greater or equals than zero.");
        Ensure.That<DomainException>(order >= 0, "Order must be greater or equals than zero.");

        Type = type;
        Legislature = legislature;
        Session = session;
        Order = order;
    }

    public static Source CreateNew(string type, int legislature, int session, int order) => 
        new(type, legislature, session, order);

    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return Type;
        yield return Legislature;
        yield return Session;
        yield return Order;
    }

    public override string ToString() => $"{Type}-{Legislature}-{Session}-{Order}";
}