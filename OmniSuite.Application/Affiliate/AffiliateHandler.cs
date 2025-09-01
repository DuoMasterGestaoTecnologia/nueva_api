using Microsoft.Extensions.Configuration;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OmniSuite.Application.Affiliate.Commands;
using OmniSuite.Application.Affiliate.Queries;
using OmniSuite.Application.Affiliate.Responses;
using OmniSuite.Application.Generic.Responses;
using OmniSuite.Domain.Entities;
using OmniSuite.Domain.Enums;
using OmniSuite.Domain.Utils;
using OmniSuite.Infrastructure.Services.KeyGenerator;
using OmniSuite.Persistence;

namespace OmniSuite.Application.Affiliate
{
    public class AffiliateHandler :
        IRequestHandler<CreateAffiliateCommand, Response<CreateAffiliateResponse>>,
        IRequestHandler<SetAffiliateInfluencerCommand, Response<bool>>,
        IRequestHandler<UpdateAffiliateCommissionCommand, Response<bool>>,
        IRequestHandler<AffiliateDashboardQuery, Response<AffiliateDashboardResponse>>
    {

        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AffiliateHandler(ApplicationDbContext context, IEmailService emailService, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<Response<CreateAffiliateResponse>> Handle(CreateAffiliateCommand request, CancellationToken cancellationToken)
        {
            var userId = UserClaimsHelper.GetUserId();

            var existing = await _context.Affiliates.FirstOrDefaultAsync(a => a.UserId == userId, cancellationToken);
            if (existing is not null)
            {
                var baseUrlExisting = _configuration["Affiliate:BaseUrl"] ?? "https://app.flowpag.com/register?ref=";
                var urlExisting = baseUrlExisting.EndsWith('=') || baseUrlExisting.EndsWith('/') || baseUrlExisting.EndsWith('?')
                    ? $"{baseUrlExisting}{existing.AffiliateCode}"
                    : $"{baseUrlExisting}?ref={existing.AffiliateCode}";
                var already = new CreateAffiliateResponse(existing.UserId, true, existing.AffiliateCode, urlExisting);
                return Response<CreateAffiliateResponse>.Ok(already);
            }

            // Generate unique affiliate code (short)
            string code;
            do
            {
                code = KeyGenerator.GenerateHexKey(4); // 8 hex chars
            } while (await _context.Affiliates.AnyAsync(a => a.AffiliateCode == code, cancellationToken));

            // Get defaults from config
            var defaultPercent = 0.10m;
            var percentStrCreate = _configuration["Affiliate:CommissionPercent"];
            if (decimal.TryParse(percentStrCreate, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var parsedCreate))
            {
                defaultPercent = parsedCreate;
            }

            var affiliate = new Affiliates
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                AffiliateCode = code,
                IsMarketUser = false,
                CommissionPercent = defaultPercent,
                TypeComission = TypeComission.TotalCommission
            };

            _context.Affiliates.Add(affiliate);
            await _context.SaveChangesAsync(cancellationToken);

            var baseUrl = _configuration["Affiliate:BaseUrl"] ?? "https://app.flowpag.com/register?ref=";
            var url = baseUrl.EndsWith('=') || baseUrl.EndsWith('/') || baseUrl.EndsWith('?')
                ? $"{baseUrl}{affiliate.AffiliateCode}"
                : $"{baseUrl}?ref={affiliate.AffiliateCode}";

            var result = new CreateAffiliateResponse(userId, true, affiliate.AffiliateCode, url);
            return Response<CreateAffiliateResponse>.Ok(result);
        }

        public async Task<Response<bool>> Handle(SetAffiliateInfluencerCommand request, CancellationToken cancellationToken)
        {
            var userId = UserClaimsHelper.GetUserId();

            var affiliate = await _context.Affiliates.FirstOrDefaultAsync(a => a.UserId == userId, cancellationToken);
            if (affiliate is null)
            {
                return Response<bool>.Fail("Afiliado não encontrado");
            }

            affiliate.IsMarketUser = request.IsMarketUser;
            await _context.SaveChangesAsync(cancellationToken);
            return Response<bool>.Ok(true);
        }

        public async Task<Response<AffiliateDashboardResponse>> Handle(AffiliateDashboardQuery request, CancellationToken cancellationToken)
        {
            var userId = UserClaimsHelper.GetUserId();

            var affiliate = await _context.Affiliates.FirstOrDefaultAsync(a => a.UserId == userId, cancellationToken);
            if (affiliate is null)
            {
                return Response<AffiliateDashboardResponse>.Fail("Afiliado não encontrado");
            }

            var baseUrl = _configuration["Affiliate:BaseUrl"] ?? "https://app.flowpag.com/register?ref=";
            var url = baseUrl.EndsWith('=') || baseUrl.EndsWith('/') || baseUrl.EndsWith('?')
                ? $"{baseUrl}{affiliate.AffiliateCode}"
                : $"{baseUrl}?ref={affiliate.AffiliateCode}";

            // Commission percent priority: value saved per affiliate; fallback to config
            var commissionPercent = affiliate.CommissionPercent > 0
                ? affiliate.CommissionPercent
                : 0.10m;
            if (commissionPercent <= 0)
            {
                var percentStr = _configuration["Affiliate:CommissionPercent"];
                if (decimal.TryParse(percentStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var parsed))
                {
                    commissionPercent = parsed;
                }
            }

            var commissionsQuery = _context.AffiliatesCommission
                .Where(c => c.AffiliatesId == affiliate.Id);

            var totalCommission = await commissionsQuery.SumAsync(c => (long?)c.Amount, cancellationToken) ?? 0L;

            // Received via withdrawals by this affiliate user (best-effort)
            var received = await _context.Withdraw
                .Where(w => w.UserId == userId && w.TransactionStatus == DepositStatusEnum.Payed)
                .SumAsync(w => (long?)w.Amount, cancellationToken) ?? 0L;

            var available = totalCommission - received;
            if (available < 0) available = 0;

            long totalDeposits = 0;
            if (commissionPercent > 0)
            {
                var depositCommission = await commissionsQuery
                    .Where(c => c.Type == TypeCommission.Deposit)
                    .SumAsync(c => (long?)c.Amount, cancellationToken) ?? 0L;
                totalDeposits = (long)Math.Round(depositCommission / commissionPercent);
            }

            var response = new AffiliateDashboardResponse
            {
                AffiliateCode = affiliate.AffiliateCode,
                AffiliateUrl = url,
                CommissionPercent = commissionPercent,
                TotalDepositsAmount = totalDeposits,
                TotalCommissionAmount = totalCommission,
                TotalReceivedAmount = received,
                AvailableToWithdraw = available
            };

            return Response<AffiliateDashboardResponse>.Ok(response);
        }

        public async Task<Response<bool>> Handle(UpdateAffiliateCommissionCommand request, CancellationToken cancellationToken)
        {
            var userId = UserClaimsHelper.GetUserId();
            var affiliate = await _context.Affiliates.FirstOrDefaultAsync(a => a.UserId == userId, cancellationToken);
            if (affiliate is null)
            {
                return Response<bool>.Fail("Afiliado não encontrado");
            }

            affiliate.CommissionPercent = request.CommissionPercent;
            affiliate.TypeComission = request.TypeComission;
            await _context.SaveChangesAsync(cancellationToken);
            return Response<bool>.Ok(true);
        }
    }
}
