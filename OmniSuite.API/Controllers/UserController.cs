namespace OmniSuite.API.Controllers
{
    [ApiController]
    [Route("user")]
    public class UserController : BaseController
    {
        [HttpPost("mfa/setup")]
        public Task<IActionResult> SetupMFA()
        {
            return SendQuery(new SetupMFAQuery());
        }

        [HttpPost("mfa/enable")]
        public Task<IActionResult> CreateMFA([FromBody] CreateMFAUserCommand command)
        {
            return SendCommand(command);
        }       

        [HttpGet("{email}")]
        public Task<IActionResult> GetByEmail(string email)
        {
            return SendQueryRaw(new UserByEmailQuery(email));
        }

        [HttpGet("logged")]
        public Task<IActionResult> GetUserLogged()
        {
            return SendQuery(new UserLoggedQuery());
        }

        [HttpPut("photo")]
        public Task<IActionResult> Update(IFormFile DocumentImageBase64)
        {
            return SendQueryRaw(new UpdatePhotoUserCommand
            {
                DocumentImageBase64 = DocumentImageBase64
            });
        }

        [HttpPost("update")]
        public Task<IActionResult> UpdateUser([FromBody] UpdateUserCommand command)
        {
            return SendCommand(command);
        }

        [HttpGet("GetUser")]
        public Task<IActionResult> GetUserById()
        {
            return SendQuery(new GetUserQuery());
        }
    }
}
