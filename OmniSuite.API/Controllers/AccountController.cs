namespace OmniSuite.API.Controllers
{
    [ApiController]
    [Route("account")]
    public class AccountController : BaseController
    {
        [AllowAnonymous]
        [HttpPost("register")]
        public Task<IActionResult> Create([FromBody] RegisterCommand command)
        {
            return SendCommand(command);
        }

        [AllowAnonymous]
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand command)
        {
            return await SendCommand(command);
        }

        [AllowAnonymous]
        [HttpPut("password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command)
        {
            return await SendCommand(command);
        }
    }
}
