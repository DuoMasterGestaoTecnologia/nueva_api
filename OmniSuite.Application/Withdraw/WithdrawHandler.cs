using OmniSuite.Application.Deposit.Responses;
using OmniSuite.Application.Withdraw.Commands;
using OmniSuite.Domain.Entities;
using OmniSuite.Domain.Utils;
using OmniSuite.Infrastructure.Services.FlowpagService;
using OmniSuite.Infrastructure.Services.FlowpagService.Request;
using OmniSuite.Infrastructure.Services.FlowpagService.Responses;

namespace OmniSuite.Application.Withdraw
{
    public class WithdrawHandler :
        IRequestHandler<WithdrawCommand, Response<bool>>
    {

        private readonly ApplicationDbContext _context;
        private readonly IFlowpagService _flowpagService;


        public WithdrawHandler(ApplicationDbContext context, IFlowpagService flowpagService)
        {
            _context = context;
            _flowpagService = flowpagService;
        }

        public async Task<Response<bool>> Handle(WithdrawCommand request, CancellationToken cancellationToken)
        {
            var userId = UserClaimsHelper.GetUserId();
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            var userBalance = await _context.UserBalance
                .FirstOrDefaultAsync(u => u.UserId == userId, cancellationToken);

            var withdrawPayload = new WithdrawPixRequest
            {
                Amount = request.Amount,
                Receiver = new ReceiverInfo
                {
                    Name = user.Name,
                    Document = UtilsDocument.formatCPFToSaveOnDb(user.Document)
                },
                Pix = new PixInfo
                {
                    PixKey = request.PixKey,
                    PixType =  request.PixType,
                }
            };

            var createDepositFlowPag = await _flowpagService.CreatePixWithdrawalAsync(withdrawPayload);

            var withdraw = new Domain.Entities.Withdraw
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Amount = UtilsMoney.ToCents(request.Amount),
                CreatedAt = DateTime.Now,
                TransactionStatus = DepositStatusEnum.Created,
                ExternalId = createDepositFlowPag.Payment.Id,
            };

           _context.Add(withdraw);

            if (userBalance is null)
            {
                // Garantia contra null (validação já cobre), mas evita NullReference
                userBalance = new UserBalance
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    TotalAmount = 0
                };

                _context.UserBalance.Add(userBalance);
            }

            userBalance.TotalAmount = userBalance.TotalAmount - withdraw.Amount;

            await _context.SaveChangesAsync();

            return Response<bool>.Ok(true);
        }
    }
}
