using OmniSuite.Application.Callback.Commands;

namespace OmniSuite.API.Controllers
{
    [ApiController]
    [Route("callback")]
    public class CallbackController : BaseController
    {
        [HttpPost]
        [AllowAnonymous]
        public Task<IActionResult> Create([FromBody] CallbackCommand command)
        {
            return SendCommand(command);
        }
    }
}
