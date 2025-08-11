using OmniSuite.Application.Generic.Responses;

namespace OmniSuite.API.Controllers
{
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        private IMediator? _mediator;

        protected IMediator Mediator =>
            _mediator ??= HttpContext.RequestServices.GetRequiredService<IMediator>();

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


