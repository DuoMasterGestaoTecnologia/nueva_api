using MediatR;

namespace OmniSuite.Application.Affiliate.Responses
{
    public record CreateAffiliateResponse(Guid UserId, bool sucess, string AffiliateCode, string AffiliateUrl);
    public class AffiliateDashboardResponse
    {
        public string AffiliateCode { get; set; } = string.Empty;
        public string AffiliateUrl { get; set; } = string.Empty;
        public decimal CommissionPercent { get; set; }
        public long TotalDepositsAmount { get; set; }
        public long TotalCommissionAmount { get; set; }
        public long TotalReceivedAmount { get; set; }
        public long AvailableToWithdraw { get; set; }
    }
}
