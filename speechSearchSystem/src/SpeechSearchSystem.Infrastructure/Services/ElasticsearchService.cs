using Nest;

using SpeechSearchSystem.Application.Services;
using SpeechSearchSystem.Domain.Entities;

namespace SpeechSearchSystem.Infrastructure.Services;

internal class ElasticsearchService : IElasticsearchService
{
    private readonly ElasticClient _client;

    public ElasticsearchService(ElasticClient client)
    {
        _client = client;
    }

    public async Task<string> IndexAsync(Speech speech, CancellationToken cancellationToken = default)
    {
        var indexResponse = await _client.IndexDocumentAsync(speech, cancellationToken);

        return !indexResponse.IsValid
            ? throw new ElasticSearchServiceException(indexResponse.DebugInformation, indexResponse.OriginalException)
            : indexResponse.Id;
    }

    public async Task<Speech> GetAsync(string id, CancellationToken cancellationToken = default)
    {
        var response = await _client
            .SourceAsync<Speech>(id, g => g.Index(Constants.ElasticSearchIndex), cancellationToken);
        return !response.IsValid
            ? throw new ElasticSearchServiceException(response.DebugInformation, response.OriginalException)
            : response.Body;
    }

    public async Task UpdateAsync(string id, Speech speech, CancellationToken cancellationToken = default)
    {
        var response = await _client
            .UpdateAsync<Speech>(id, u => u.Index(Constants.ElasticSearchIndex).Doc(speech), cancellationToken);
        if (!response.IsValid)
        {
            throw new ElasticSearchServiceException(response.DebugInformation, response.OriginalException);
        }
    }
}