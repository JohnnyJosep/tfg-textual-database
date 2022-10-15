using Newtonsoft.Json;

using SpeechSearchSystem.Domain.JsonConverters;
using SpeechSearchSystem.Domain.ValueObjects.Base;

namespace SpeechSearchSystem.Domain.ValueObjects;

[JsonConverter(typeof(InterruptionJsonConverter))]
public class Interruption : ValueObject
{
    public string Value { get; }

    private Interruption(string value)
    {
        Ensure.That<DomainException>(!string.IsNullOrEmpty(value), "Interruption can not be null or empty.");
        Value = value;
    }

    public static Interruption FromString(string value) => new(value);

    public static implicit operator string(Interruption interruption) => interruption.Value;
    public static explicit operator Interruption(string value) => new(value);

    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}