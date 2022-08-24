using Microsoft.Extensions.DependencyInjection;

namespace TextualDatabase.Infrastructure;
public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services) => 
        services.AddStackExchangeRedisCache(options => options.Configuration = "redis:6379");
    
}
