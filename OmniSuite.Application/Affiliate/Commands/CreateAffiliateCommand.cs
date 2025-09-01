using MediatR;
using OmniSuite.Application.Affiliate.Responses;
using OmniSuite.Application.Generic.Responses;

namespace OmniSuite.Application.Affiliate.Commands
{
    public record CreateAffiliateCommand() : IRequest<Response<CreateAffiliateResponse>>;
    public record SetAffiliateInfluencerCommand(bool IsMarketUser) : IRequest<Response<bool>>;
    public record UpdateAffiliateCommissionCommand(decimal CommissionPercent, TypeComission TypeComission) : IRequest<Response<bool>>;
}
