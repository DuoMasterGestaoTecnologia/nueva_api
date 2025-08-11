using OmniSuite.Application.Deposit.Commands;
using OmniSuite.Application.Deposit.Queries;

namespace OmniSuite.API.Controllers
{
    [ApiController]
    [Route("deposit")]
    public class DepositController : BaseController
    {

        [HttpGet]
        public Task<IActionResult> Get([FromQuery] DepositQuery query)
        {
            return SendQuery(query);

        }

        [HttpPost]
        public Task<IActionResult> Create([FromBody] DepositCommand command)
        {
            return SendCommand(command);

        }
    }
}
