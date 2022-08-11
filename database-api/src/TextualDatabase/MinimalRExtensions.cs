using MediatR;

using TextualDatabase.Handlers.Base;

namespace TextualDatabase;

internal static class MinimaRExtenions 
{
    internal static RouteHandlerBuilder MapMediateGet<TRequest>(this WebApplication app, string pattern) 
        where TRequest : IHttpRequest => 
        app.MapGet(pattern, async(IMediator mediator, [AsParameters] TRequest request) => await mediator.Send(request));
    
}