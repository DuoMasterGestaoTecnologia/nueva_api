using Microsoft.AspNetCore.Mvc;
using OmniSuite.Application.Affiliate.Commands;
using OmniSuite.Application.Withdraw.Commands;

namespace OmniSuite.API.Controllers
{
    [ApiController]
    [Route("affiliate")]
    public class AffiliateController : BaseController
    {
        [HttpPost]
        public Task<IActionResult> Create([FromBody] CreateAffiliateCommand command)
        {
            return SendCommand(command);

        }
    }
}
