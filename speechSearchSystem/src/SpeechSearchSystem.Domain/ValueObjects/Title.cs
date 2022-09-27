using System.Text.Json.Serialization;

using SpeechSearchSystem.Domain.JsonConverters;
using SpeechSearchSystem.Domain.ValueObjects.Base;

namespace SpeechSearchSystem.Domain.ValueObjects;

[JsonConverter(typeof(TitleJsonConverter))]
[Newtonsoft.Json.JsonConverter(typeof(TitleNewtonsoftJsonConverter))]
public class Title : ValueObject
{
    public string Value { get; }

    private Title(string title)
    {
        Ensure.That<DomainException>(!string.IsNullOrWhiteSpace(title), "Title can not be null or white space.");
        Value = title;
    }
    

    public static Title FromString(string value) => new(value);

    public static implicit operator string(Title title) => title.Value;
    public static explicit operator Title(string value) => new(value);
    
    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}