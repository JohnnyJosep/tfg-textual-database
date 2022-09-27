using SpeechSearchSystem.Domain.ValueObjects.Base;

namespace SpeechSearchSystem.Domain.ValueObjects;

/// <summary>
/// Validate TagSet according to <see href="https://freeling-user-manual.readthedocs.io/en/v4.2/tagsets/tagset-es/"></see>
/// </summary>
public class TagSet : ValueObject
{
    public string PartOfSpeech { get; }

    private TagSet(ReadOnlySpan<char> values)
    {
        if (values.Length == 0) throw new DomainException("Values can not be empty or null");

        var errorMessage = values[0] switch
        {
            Adjective.Value => Adjective.IsValid(values[1..]),
            Conjunction.Value => Conjunction.IsValid(values[1..]),
            Determiner.Value => Determiner.IsValid(values[1..]),
            Noun.Value => Noun.IsValid(values[1..]),
            Pronoun.Value => Pronoun.IsValid(values[1..]),
            Adverb.Value => Adverb.IsValid(values[1..]),
            Adposition.Value => Adposition.IsValid(values[1..]),
            Verb.Value => Verb.IsValid(values[1..]),
            Number.Value => Number.IsValid(values[1..]),
            Date.Value => Date.IsValid(values[1..]),
            Interjection.Value => Interjection.IsValid(values[1..]),
            Punctuation.Value => Punctuation.IsValid(values[1..]),
            _ => "Category not found on TagSet"
        };

        if (string.IsNullOrEmpty(errorMessage)) throw new DomainException(errorMessage);

        PartOfSpeech = values.ToString();
    }

    public static TagSet FromRaw(string value) => new(value);

    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return PartOfSpeech;
    }

    public override string ToString() => PartOfSpeech;
}

internal static class Adjective
{
    public const char Value = 'A';

    public static string IsValid(ReadOnlySpan<char> values)
    {
        var error = AttributeValidator.ValidateOrdinalQualifierOrPossessive(values[0]);
        if (!string.IsNullOrEmpty(error)) return error;

        error = AttributeValidator.ValidateSuperlativeOrEvaluative(values[1]);
        if (!string.IsNullOrEmpty(error)) return error;

        error = AttributeValidator.ValidateFeminineMasculineOrCommon(values[2]);
        if (!string.IsNullOrEmpty(error)) return error;

        error = AttributeValidator.ValidateSingularPluralOrInvariable(values[3]);
        if (!string.IsNullOrEmpty(error)) return error;

        error = AttributeValidator.ValidateFirstSecondOrThird(values[4]);
        if (!string.IsNullOrEmpty(error)) return error;

        error = AttributeValidator.ValidateSingularPluralOrInvariable(values[5]);
        if (!string.IsNullOrEmpty(error)) return error;

        return string.Empty;
    }
}

internal static class Conjunction
{
    public const char Value = 'C';

    public static string IsValid(ReadOnlySpan<char> values)
    {
        var error = AttributeValidator.ValidateCoordinatingOrSubordinating(values[0]);
        return !string.IsNullOrEmpty(error) ? error : string.Empty;
    }
}

internal static class Determiner
{
    public const char Value = 'D';

    public static string IsValid(ReadOnlySpan<char> values)
    {
        var error = AttributeValidator
            .ValidateArticleDemonstrativeIndefinitePossessiveInterrogativeOrExclamatory(values[0]);
        if (!string.IsNullOrEmpty(error)) return error;

        error = AttributeValidator.ValidateFirstSecondOrThird(values[1]);
        if (!string.IsNullOrEmpty(error)) return error;

        error = AttributeValidator.ValidateFeminineMasculineOrCommon(values[2]);
        if (!string.IsNullOrEmpty(error)) return error;

        error = AttributeValidator.ValidateSingularPluralOrInvariable(values[3]);
        if (!string.IsNullOrEmpty(error)) return error;

        error = AttributeValidator.ValidateSingularPluralOrInvariable(values[4]);
        if (!string.IsNullOrEmpty(error)) return error;

        return string.Empty;
    }
}

internal static class Noun
{
    public const char Value = 'N';

    public static string IsValid(ReadOnlySpan<char> values)
    {
        var error = AttributeValidator.ValidateCommonOrProper(values[0]);
        if (!string.IsNullOrEmpty(error)) return error;

        error = AttributeValidator.ValidateFeminineMasculineOrCommon(values[1]);
        if (!string.IsNullOrEmpty(error)) return error;

        error = AttributeValidator.ValidateSingularPluralOrInvariable(values[2]);
        if (!string.IsNullOrEmpty(error)) return error;

        error = AttributeValidator.ValidateSingularPluralOrInvariable(values[2]);
        if (!string.IsNullOrEmpty(error)) return error;

        error = AttributeValidator.ValidatePersonLocationOrganizationOrOther(values[3]);
        if (!string.IsNullOrEmpty(error)) return error;

        error = AttributeValidator.ValidateEvaluative(values[4]);
        if (!string.IsNullOrEmpty(error)) return error;

        return string.Empty;
    }
}

internal static class Pronoun
{
    public const char Value = 'P';

    public static string IsValid(ReadOnlySpan<char> values)
    {
        var error = AttributeValidator
            .ValidateDemonstrativeExclamatoryIndefinitePersonalRelativeOrInterrogative(values[0]);
        if (!string.IsNullOrEmpty(error)) return error;

        error = AttributeValidator.ValidateFirstSecondOrThird(values[1]);
        if (!string.IsNullOrEmpty(error)) return error;

        error = AttributeValidator.ValidateFeminineMasculineOrCommon(values[2]);
        if (!string.IsNullOrEmpty(error)) return error;

        error = AttributeValidator.ValidateSingularPluralOrInvariable(values[3]);
        if (!string.IsNullOrEmpty(error)) return error;

        error = AttributeValidator.ValidateNominativeAccusativeDativeOrOblique(values[4]);
        if (!string.IsNullOrEmpty(error)) return error;

        error = AttributeValidator.ValidateYes(values[5]);
        if (!string.IsNullOrEmpty(error)) return error;

        return string.Empty;
    }
}

internal static class Adverb
{
    public const char Value = 'R';

    public static string IsValid(ReadOnlySpan<char> values) => AttributeValidator.ValidateNegativeOrGeneral(values[0]);
    
}

internal static class Adposition
{
    public const char Value = 'S';

    public static string IsValid(ReadOnlySpan<char> values) => AttributeValidator.ValidatePreposition(values[0]);
    
}

internal static class Verb
{
    public const char Value = 'V';

    public static string IsValid(ReadOnlySpan<char> values)
    {
        var error = AttributeValidator.ValidateMainAuxiliaryOrSemiAuxiliary(values[0]);
        if (!string.IsNullOrEmpty(error)) return error;

        error = AttributeValidator.ValidateIndicativeSubjectiveImperativeParticipleGerundOrInfinitive(values[1]);
        if (!string.IsNullOrEmpty(error)) return error;

        error = AttributeValidator.ValidatePresentImperfectFuturePasetOrConditional(values[2]);
        if (!string.IsNullOrEmpty(error)) return error;

        error = AttributeValidator.ValidateFirstSecondOrThird(values[3]);
        if (!string.IsNullOrEmpty(error)) return error;

        error = AttributeValidator.ValidateSingularOrPlural(values[4]);
        if (!string.IsNullOrEmpty(error)) return error;

        error = AttributeValidator.ValidateFeminineMasculineOrCommon(values[5]);
        if (!string.IsNullOrEmpty(error)) return error;

        return string.Empty;
    }
}

internal static class Number
{
    public const char Value = 'Z';

    public static string IsValid(ReadOnlySpan<char> values) => values[0] switch
    {
        'd' or 'm' or 'p' or 'u' => string.Empty,
        _ => $"Number type out of scope (found {values[0]})"
    };
}

internal static class Date
{
    public const char Value = 'W';

    public static string IsValid(ReadOnlySpan<char> values) => string.Empty;
}

internal static class Interjection
{
    public const char Value = 'I';

    public static string IsValid(ReadOnlySpan<char> values) => string.Empty;
}

internal static class Punctuation
{
    public const char Value = 'F';

    public static string IsValid(ReadOnlySpan<char> values) => values.ToString() switch
    {
        "d" or "c" or "lt" or "la" or "s" or "at" or "aa" or "g" or "z" or "pt" or "pa" or "t" or "p" or "it" or "ia"
            or "e" or "rc" or "ra" or "x" or "h" or "ct" or "ca" => string.Empty,
        _ => $"Punctuation type out of scope (found {values.ToString()})"
    };
}

internal static class AttributeValidator
{
    public static string ValidateOrdinalQualifierOrPossessive(char c) => c switch
    {
        'O' or 'Q' or 'P' => string.Empty,
        _ => $"Type adjective value out of scope (found {c})"
    };

    public static string ValidateSuperlativeOrEvaluative(char c) => c switch
    {
        'S' or 'V' => string.Empty,
        _ => $"Degree value out of scope (found {c})"
    };

    public static string ValidateFeminineMasculineOrCommon(char c) => c switch
    {
        'M' or 'F' or 'C' => string.Empty,
        _ => $"Genre value out of scope (found {c})"
    };

    public static string ValidateSingularPluralOrInvariable(char c) => c switch
    {
        'S' or 'P' or 'N' => string.Empty,
        _ => $"Number value out of scope (found {c})"
    };
    public static string ValidateSingularOrPlural(char c) => c switch
    {
        'S' or 'P' => string.Empty,
        _ => $"Number value out of scope (found {c})"
    };

    public static string ValidateFirstSecondOrThird(char c) => c switch
    {
        '1' or '2' or '3' => string.Empty,
        _ => $"Possessor person value out of scope (found {c})"
    };

    public static string ValidateCoordinatingOrSubordinating(char c) => c switch
    {
        'C' or 'S' => string.Empty,
        _ => $"Type conjunction value out of scope (found {c})"
    };

    public static string ValidateArticleDemonstrativeIndefinitePossessiveInterrogativeOrExclamatory(char c) => c switch
    {
        'A' or 'D' or 'I' or 'P' or 'T' or 'E' => string.Empty,
        _ => $"Type determiner value out of scope (found {c})"
    };

    public static string ValidateCommonOrProper(char c) => c switch
    {
        'C' or 'P' => string.Empty,
        _ => $"Type noun value out of scope (found {c})"
    };

    public static string ValidatePersonLocationOrganizationOrOther(char c) => c switch
    {
        'P' or 'G' or 'O' or 'V' => string.Empty,
        _ => $"Type neClass value out of scope (found {c})"
    };

    public static string ValidateEvaluative(char c) => c switch
    {
        'V' => string.Empty,
        _ => $"Noun degree value out of scope (found {c})"
    };

    public static string ValidateDemonstrativeExclamatoryIndefinitePersonalRelativeOrInterrogative(char c) => c switch
    {
        'D' or 'E' or 'I' or 'P' or 'R' or 'T' => string.Empty,
        _ => $"Type pronoun value out of scope (found {c})"
    };

    public static string ValidateNominativeAccusativeDativeOrOblique(char c) => c switch
    {
        'N' or 'A' or 'D' or 'O' => string.Empty,
        _ => $"Case value out of scope (found {c})"
    };

    public static string ValidateYes(char c) => c switch
    {
        'P' => string.Empty,
        _ => $"Polite value out of scope (found {c})"
    };

    public static string ValidateNegativeOrGeneral(char c) => c switch
    {
        'N' or 'G' => string.Empty,
        _ => $"Type adverb value out of scope (found {c})"
    };

    public static string ValidatePreposition(char c) => c switch
    {
        'P' => string.Empty,
        _ => $"Type adposition value out of scope (found {c})"
    };

    public static string ValidateMainAuxiliaryOrSemiAuxiliary(char c) => c switch
    {
        'M' or 'A' or 'S' => string.Empty,
        _ => $"Type verb value out of scope (found {c})"
    };

    public static string ValidateIndicativeSubjectiveImperativeParticipleGerundOrInfinitive(char c) => c switch
    {
        'I' or 'S' or 'M' or 'P' or 'G' or 'N' => string.Empty,
        _ => $"Mood verb value out of scope (found {c})"
    };

    public static string ValidatePresentImperfectFuturePasetOrConditional(char c) => c switch
    {
        'P' or 'I' or 'F' or 'S' or 'C' => string.Empty,
        _ => $"Tense verb value out of scope (found {c})"
    };
}