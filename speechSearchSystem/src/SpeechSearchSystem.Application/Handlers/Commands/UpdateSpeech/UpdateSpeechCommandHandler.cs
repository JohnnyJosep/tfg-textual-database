using MediatR;

using SpeechSearchSystem.Application.Services;
using SpeechSearchSystem.Domain.Entities;

namespace SpeechSearchSystem.Application.Handlers.Commands.UpdateSpeech;

public record UpdateSpeechCommand(string Id, string MorphologicalAnalysisRaw) : IRequest;

internal class UpdateSpeechCommandHandler : IRequestHandler<UpdateSpeechCommand>
{
    private readonly IElasticsearchService _elasticsearchService;

    public UpdateSpeechCommandHandler(IElasticsearchService elasticsearchService)
    {
        _elasticsearchService = elasticsearchService;
    }

    public async Task<Unit> Handle(UpdateSpeechCommand request, CancellationToken cancellationToken)
    {
        var speech = await _elasticsearchService.GetAsync(request.Id, cancellationToken);
        speech.AddMorphologicalAnalysis(request.MorphologicalAnalysisRaw);
        await _elasticsearchService.UpdateAsync(request.Id, speech, cancellationToken);

        return Unit.Value;
    }
}