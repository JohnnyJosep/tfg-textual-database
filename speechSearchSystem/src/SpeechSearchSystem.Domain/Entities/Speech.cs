using Ardalis.GuardClauses;

using SpeechSearchSystem.Domain.Enums;

namespace SpeechSearchSystem.Domain.Entities
{
    public class Speech
    {
        public string Id { get; }
        public int Legislature { get; }
        public int Session { get; }
        public string Title { get; }
        public int Order { get; }
        public Gender Gender { get; }
        public DateOnly Date { get; }
        public string Source { get; }
        public string Text { get; }

        public Speech(string id, int legislature, int session, string title, int order, Gender gender, DateOnly date, string source, string text)
        {
            Id = id;
            Legislature = Guard.Against.NegativeOrZero(legislature, nameof(legislature));
            Session = Guard.Against.NegativeOrZero(session, nameof(session));
            Title = Guard.Against.NullOrWhiteSpace(title, nameof(title));
            Order = Guard.Against.Negative(order, nameof(order));
            Gender = gender;
            Date = date;
            Source = Guard.Against.NullOrWhiteSpace(source, nameof(source));
            Text = Guard.Against.NullOrWhiteSpace(text, nameof(text));
        }
    }
}
