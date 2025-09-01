using Microsoft.AspNetCore.Mvc;
using OmniSuite.Application.Affiliate.Commands;
using OmniSuite.Application.Affiliate.Queries;
using OmniSuite.Application.Affiliate.Responses;

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

        [HttpPost("influencer")]
        public Task<IActionResult> SetInfluencer([FromBody] SetAffiliateInfluencerCommand command)
        {
            return SendCommand(command);
        }

        [HttpGet("dashboard")]
        public Task<IActionResult> Dashboard([FromQuery] AffiliateDashboardQuery query)
        {
            return SendQuery(query);
        }

        [HttpPut("commission")]
        public Task<IActionResult> UpdateCommission([FromBody] UpdateAffiliateCommissionCommand command)
        {
            return SendCommand(command);
        }
    }
}
