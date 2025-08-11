using OmniSuite.Application.Affiliate.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmniSuite.Application.Affiliate.Commands
{
    public record CreateAffiliateCommand(string Secret, string Code) : IRequest<Response<CreateAffiliateResponse>>;
}
