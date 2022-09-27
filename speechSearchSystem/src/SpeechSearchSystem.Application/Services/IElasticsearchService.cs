using SpeechSearchSystem.Domain.Entities;

namespace SpeechSearchSystem.Application.Services;

public interface IElasticsearchService
{
    Task<string> IndexAsync(Speech speech, CancellationToken cancellationToken = default);

    Task<Speech> GetAsync(string id, CancellationToken cancellationToken = default);

    Task UpdateAsync(Speech speech, CancellationToken cancellationToken = default);
}