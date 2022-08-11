using MediatR;

namespace TextualDatabase.Handlers.Base;

internal interface IHttpRequest : IRequest<IResult> {}