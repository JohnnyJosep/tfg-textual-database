using System.Text.Json.Serialization;

using SpeechSearchSystem.Domain.JsonConverters;
using SpeechSearchSystem.Domain.ValueObjects.Base;

namespace SpeechSearchSystem.Domain.ValueObjects;

[JsonConverter(typeof(TextJsonConverter))]
[Newtonsoft.Json.JsonConverter(typeof(TextNewtonsoftJsonConverter))]
public class Text : ValueObject
{
    public string Value { get; }

    private Text(string value)
    {
        Ensure.That<DomainException>(!string.IsNullOrEmpty(value), "Text can not be null or empty.");
        Value = value;
    }

    public static Text FromString(string value) => new(value);

    public static implicit operator string(Text text) => text.Value;
    public static explicit operator Text(string value) => new(value);

    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}