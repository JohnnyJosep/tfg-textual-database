using MediatR;

using Microsoft.AspNetCore.Mvc;

using SpeechSearchSystem.Application.Handlers.Commands.CreateSpeech;
using SpeechSearchSystem.Application.Handlers.Commands.UpdateSpeech;
using SpeechSearchSystem.Domain;
using SpeechSearchSystem.Infrastructure.Services;

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
            try
            {
                var response = await _mediator.Send(command, cancellationToken);
                return Ok(response);
            }
            catch (DomainException de)
            {
                return BadRequest(de.Message);
            }
            catch (ElasticSearchServiceException ese)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ese.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, UpdateSpeechCommand command, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest();

            try
            {
                await _mediator.Send(command, cancellationToken);
                return NoContent();
            }
            catch (DomainException de)
            {
                return BadRequest(de.Message);
            }
            catch (ElasticSearchServiceException ese)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ese.Message);
            }
        }
    }
}
