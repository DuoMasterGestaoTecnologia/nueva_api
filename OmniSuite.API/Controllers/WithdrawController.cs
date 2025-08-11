using OmniSuite.Application.Deposit.Queries;
using OmniSuite.Application.Withdraw.Commands;

namespace OmniSuite.API.Controllers
{
    [ApiController]
    [Route("withdraw")]
    public class WithdrawController : BaseController
    {
        [HttpGet]
        public Task<IActionResult> Get([FromQuery] DepositQuery query)
        {
            return SendQuery(query);

        }

        [HttpPost]
        public Task<IActionResult> Create([FromBody] WithdrawCommand command)
        {
            return SendCommand(command);

        }
    }
}
