using MediatR;

namespace TextualDatabase;

internal static class MinimaRExtenions
{
    internal static RouteHandlerBuilder MapMediateGet<TRequest, TResponse>(this WebApplication app, string pattern)
        where TRequest : IRequest<TResponse> =>
        app.MapGet(pattern, async (IMediator mediator, [AsParameters] TRequest request) =>
        {
            var response = await mediator.Send(request);
            return Results.Ok(response);
        });
    internal static RouteHandlerBuilder MapMediatePost<TRequest, TResponse>(this WebApplication app, string pattern)
        where TRequest : IRequest<TResponse> =>
        app.MapPost(pattern, async (IMediator mediator, [AsParameters] TRequest request) =>
        {
            var response = await mediator.Send(request);
            return Results.Ok(response);
        });

}