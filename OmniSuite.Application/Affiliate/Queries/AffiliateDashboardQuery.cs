using MediatR;
using OmniSuite.Application.Affiliate.Responses;
using OmniSuite.Application.Generic.Responses;

namespace OmniSuite.Application.Affiliate.Queries
{
    public record AffiliateDashboardQuery() : IRequest<Response<AffiliateDashboardResponse>>;
}

