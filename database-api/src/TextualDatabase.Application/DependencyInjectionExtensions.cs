using MediatR;

using Microsoft.Extensions.DependencyInjection;

namespace TextualDatabase.Application;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services) => 
        services.AddMediatR(x => x.AsScoped(), typeof(IApplicationMark));
        
}
