using System.Reflection;

using MediatR;

using Microsoft.Extensions.DependencyInjection;

namespace SpeechSearchSystem.Application;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(Assembly.GetExecutingAssembly());

        return services;
    }
}