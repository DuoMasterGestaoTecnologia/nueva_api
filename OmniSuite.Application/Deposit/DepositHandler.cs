using OmniSuite.Application.Deposit.Commands;
using OmniSuite.Application.Deposit.Queries;
using OmniSuite.Application.Deposit.Responses;
using OmniSuite.Domain.Utils;
using OmniSuite.Infrastructure.Services.FlowpagService;
using OmniSuite.Infrastructure.Services.FlowpagService.Request;

namespace OmniSuite.Application.Deposit
{
    public class DepositHandler : 
        IRequestHandler<DepositCommand, Response<DepositResponse>>,
        IRequestHandler<DepositQuery, Response<PaginatedResult<DepositQueryResponse>>>
    {
        private readonly ApplicationDbContext _context;
        private readonly IFlowpagService _flowpagService;

        public DepositHandler(ApplicationDbContext context, IFlowpagService flowpagService)
        {
            _context = context;
            _flowpagService = flowpagService;
        }
        public async Task<Response<DepositResponse>> Handle(DepositCommand request, CancellationToken cancellationToken)
        {
            var userId = UserClaimsHelper.GetUserId();
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            var depositPayload = new PixDepositRequest
            {
                Amount = request.Amount,
                Payer = new PayerInfo
                {
                    Name = user.Name,
                    Document = UtilsDocument.formatCPFToSaveOnDb(request.Document)
                }
            };

            var createDepositFlowPag = await _flowpagService.CreatePixDepositAsync(depositPayload);

            var deposit = new Domain.Entities.Deposit
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Amount = UtilsMoney.ToCents(request.Amount),
                CreatedAt = DateTime.Now,
                TransactionStatus = DepositStatusEnum.Created,
                PaymentMethod = DepositPaymentTypeEnum.Pix,
                ExternalId = createDepositFlowPag.Payment.Id,
                PaymentCode = createDepositFlowPag.Payment.PixCopyPasteCode
            };

            _context.Add(deposit);
            
            await _context.SaveChangesAsync();
            
            return Response<DepositResponse>.Ok(new DepositResponse
            {
                Id = deposit.Id,
                PixCopyPasteCode = createDepositFlowPag.Payment.PixCopyPasteCode,
                QrCodeBase64 = createDepositFlowPag.Payment.QrCodeBase64
            });
        }

        public async Task<Response<PaginatedResult<DepositQueryResponse>>> Handle(DepositQuery request, CancellationToken cancellationToken)
        {
            var userId = UserClaimsHelper.GetUserId();

            var query = _context.Deposit
                .Where(d => d.UserId == userId)
                .AsNoTracking()
                .OrderByDescending(x => x.CreatedAt)
                .Select(d => new DepositQueryResponse
                {
                    Id = d.Id,
                    Amount = d.Amount,
                    PaymentMethod = d.PaymentMethod,
                    TransactionStatus = d.TransactionStatus,
                    CreatedAt = d.CreatedAt,
                    PaymentCode = d.PaymentCode,
                });

            var result = await query.PaginateAsync(request.Page, request.PageSize, cancellationToken);

            return Response<PaginatedResult<DepositQueryResponse>>.Ok(result);
        }
    }
}
