using OmniSuite.Application.Generic.Responses;

namespace OmniSuite.API.Controllers
{
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        protected IMediator? _mediator;

        protected virtual IMediator Mediator =>
            _mediator ??= HttpContext?.RequestServices?.GetRequiredService<IMediator>() ?? 
                         throw new InvalidOperationException("HttpContext is not available or IMediator is not configured");

        // Method to set mediator for testing purposes
        public void SetMediator(IMediator mediator)
        {
            _mediator = mediator;
        }

        // Method to get mediator for testing purposes
        public IMediator? GetMediator()
        {
            return _mediator;
        }

        protected async Task<IActionResult> SendCommand<T>(IRequest<Response<T>> command)
        {
            var result = await Mediator.Send(command);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        protected async Task<IActionResult> SendQuery<T>(IRequest<Response<T>> query)
        {
            var result = await Mediator.Send(query);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        protected async Task<IActionResult> SendQueryRaw<T>(IRequest<T> query)
        {
            var result = await Mediator.Send(query);
            return Ok(result);
        }
    }
}


