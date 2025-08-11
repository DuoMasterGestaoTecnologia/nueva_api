namespace OmniSuite.API.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthenticationController : BaseController
    {
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
        {
            return await SendCommand(command);
        }

        [HttpPost("refresh")]
        public Task<IActionResult> Refresh([FromBody] RefreshTokenCommand command)
        {
            return SendCommand(command);
        }

        [HttpDelete("logout")]
        public Task<IActionResult> Logout([FromBody] LogoutCommand command)
        {
            return SendCommand(command);
        }
    }
}
