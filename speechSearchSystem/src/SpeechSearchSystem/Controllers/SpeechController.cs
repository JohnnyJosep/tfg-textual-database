using MediatR;

using Microsoft.AspNetCore.Mvc;

using SpeechSearchSystem.Application.Handlers.Commands.CreateSpeech;
using SpeechSearchSystem.Application.Handlers.Commands.UpdateSpeech;

namespace SpeechSearchSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpeechController : ControllerBase
    {
        private readonly ISender _mediator;

        public SpeechController(ISender mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Post(CreateSpeechCommand command, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(command, cancellationToken);
            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, UpdateSpeechCommand command, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest();

            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }
    }
}
