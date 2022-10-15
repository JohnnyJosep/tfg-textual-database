using Elasticsearch.Net;

using Microsoft.Extensions.DependencyInjection;

using Nest;
using Nest.JsonNetSerializer;

using SpeechSearchSystem.Application.Services;
using SpeechSearchSystem.Domain.Entities;
using SpeechSearchSystem.Infrastructure.Services;

namespace SpeechSearchSystem.Infrastructure;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        var pool = new SingleNodeConnectionPool(new Uri(Constants.ElasticSearchHost));
        var settings = new ConnectionSettings(pool, JsonNetSerializer.Default)
            .DisableDirectStreaming()
            .DefaultIndex(Constants.ElasticSearchIndex)
            .EnableApiVersioningHeader();
        var client = new ElasticClient(settings);
        services.AddSingleton(client);

        services.AddTransient<IElasticsearchService, ElasticsearchService>();
        services.AddTransient<IMessageBroker, RabbitMqMessageBroker>();
        return services;
    }
}