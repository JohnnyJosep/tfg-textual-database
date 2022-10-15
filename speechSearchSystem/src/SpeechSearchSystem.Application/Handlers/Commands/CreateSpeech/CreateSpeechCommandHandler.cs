using System.Text.Json.Serialization;

using MediatR;

using SpeechSearchSystem.Application.JsonConverters;
using SpeechSearchSystem.Application.Services;
using SpeechSearchSystem.Contracts;
using SpeechSearchSystem.Domain.Entities;
using SpeechSearchSystem.Domain.Enums;
using SpeechSearchSystem.Domain.ValueObjects;

namespace SpeechSearchSystem.Application.Handlers.Commands.CreateSpeech;

public record CreateSpeechCommand(
        [property: JsonPropertyName("title")]
        string Title,
        [property: JsonPropertyName("text")]
        string Text,
        [property:JsonPropertyName("celebrated_at")]
        [property:JsonConverter(typeof(DateOnlyJsonConverter))]
        DateOnly CelebratedAt,
        [property:JsonPropertyName("source_type")]
        string SourceType,
        [property: JsonPropertyName("source_legislature")]
        int SourceLegislature,
        [property:JsonPropertyName("source_session")]
        int SourceSession,
        [property:JsonPropertyName("source_order")]
        int SourceOrder,
        [property:JsonPropertyName("author_name")]
        string AuthorName,
        [property:JsonPropertyName("author_surname")]
        string AuthorSurname,
        [property:JsonPropertyName("author_group")]
        string AuthorGroup,
        [property:JsonPropertyName("author_formation")]
        string AuthorFormation,
        [property:JsonPropertyName("author_gender")]
        [property:JsonConverter(typeof(JsonStringEnumConverter))]
        Gender AuthorGender,
        [property:JsonPropertyName("total_interruptions")]
        int TotalInterruptions,
        [property:JsonPropertyName("interruptions")]
        string[] Interruptions)
    : IRequest<string>;

internal class CreateSpeechCommandHandler : IRequestHandler<CreateSpeechCommand, string>
{
    private readonly IElasticsearchService _elasticsearchService;
    private readonly IMessageBroker _messageBroker;

    public CreateSpeechCommandHandler(IElasticsearchService elasticsearchService, IMessageBroker messageBroker)
    {
        _elasticsearchService = elasticsearchService;
        _messageBroker = messageBroker;
    }

    public async Task<string> Handle(CreateSpeechCommand request, CancellationToken cancellationToken)
    {
        var speech = new Speech(
            (Title)request.Title,
            (Text)request.Text,
            request.CelebratedAt,
            Source.CreateNew(request.SourceType, request.SourceLegislature, request.SourceSession, request.SourceOrder),
            Author.CreateNew(request.AuthorName, request.AuthorSurname, request.AuthorGroup, request.AuthorFormation, request.AuthorGender),
            request.TotalInterruptions, request.Interruptions.Select(Interruption.FromString).ToArray());

        try
        {
            var existingId = await _elasticsearchService.ExistingIdBySource(speech.Source, cancellationToken);
            if (!string.IsNullOrEmpty(existingId))
            {
                return existingId;
            }
        }
        catch (Exception ex)
        {
            if (ex.InnerException is not null) throw;
        }

        var id = await _elasticsearchService.IndexAsync(speech, cancellationToken);
        _messageBroker.SendMessage(new CreatedSpeechMessage(id, request.Text));

        return id;
    }
}