using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmniSuite.Application.Affiliate.Responses
{
    public record CreateAffiliateResponse(Guid UserId, bool sucess);
}
