using System.Text.Json.Serialization;

using MediatR;

using SpeechSearchSystem.Application.JsonConverters;
using SpeechSearchSystem.Application.Services;
using SpeechSearchSystem.Domain.Entities;
using SpeechSearchSystem.Domain.ValueObjects;

namespace SpeechSearchSystem.Application.Handlers.Commands.CreateSpeech;

public record CreateSpeechCommand(
    string Title, string Text, 
    string Source, int Legislature, int Session, int Order,
    string Name, string Surname, string Group, string Formation, 
    [property:JsonConverter(typeof(DateOnlyJsonConverter))]DateOnly EntryDate, [property: JsonConverter(typeof(DateOnlyJsonConverter))] DateOnly? LeavingDate) 
    : IRequest<string>;

internal record CreatedSpeechMessage(string Id, string Text);

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
            Source.CreateNew(request.Source, request.Legislature, request.Session, request.Order), 
            Author.CreateNew(request.Name, request.Surname, request.Group, request.Formation, request.EntryDate, request.LeavingDate));
        
        var id = await _elasticsearchService.IndexAsync(speech, cancellationToken);
        
        _messageBroker.SendMessage(new CreatedSpeechMessage(id, request.Text));

        return id;
    }
}