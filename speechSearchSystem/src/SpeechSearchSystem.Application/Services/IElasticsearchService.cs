using SpeechSearchSystem.Domain.Entities;
using SpeechSearchSystem.Domain.ValueObjects;

namespace SpeechSearchSystem.Application.Services;

public interface IElasticsearchService
{
    Task<string> ExistingIdBySource(Source source, CancellationToken cancellationToken = default);

    Task<string> IndexAsync(Speech speech, CancellationToken cancellationToken = default);

    Task<Speech> GetAsync(string id, CancellationToken cancellationToken = default);

    Task UpdateAsync(string id, Speech speech, CancellationToken cancellationToken = default);
}