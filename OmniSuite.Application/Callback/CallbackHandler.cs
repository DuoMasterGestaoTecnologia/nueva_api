using OmniSuite.Application.Callback.Commands;
using OmniSuite.Application.Generic.Responses;
using OmniSuite.Domain.Entities;
using System.Transactions;

namespace OmniSuite.Application.Callback
{
    public class CallbackHandler : IRequestHandler<CallbackCommand, Response<bool>>
    {
        private readonly ApplicationDbContext _context;

        public CallbackHandler(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public async Task<Response<bool>> Handle(CallbackCommand request, CancellationToken cancellationToken)
        {
            var deposit = await _context.Deposit
                .FirstOrDefaultAsync(d => d.ExternalId == request.Id, cancellationToken);

            var userBalance = await _context.UserBalance
                .FirstOrDefaultAsync(u => u.UserId == deposit.UserId, cancellationToken);


            deposit.TransactionStatus = request.Status switch
            {
                "paid" => DepositStatusEnum.Payed,
                "reproved" => DepositStatusEnum.Reproved,
                "rejected" => DepositStatusEnum.Error,
                _ => deposit.TransactionStatus
            };

            // Creditar saldo somente quando efetivamente pago
            if (deposit.TransactionStatus == DepositStatusEnum.Payed)
            {
                if (userBalance is null)
                {
                    userBalance = new UserBalance
                    {
                        Id = Guid.NewGuid(),
                        UserId = deposit.UserId,
                        TotalAmount = deposit.Amount
                    };

                    _context.UserBalance.Add(userBalance);
                }
                else
                {
                    userBalance.TotalAmount += deposit.Amount;
                }
            }
                



            await _context.SaveChangesAsync(cancellationToken);

            return Response<bool>.Ok(true);
        }
    }    
}
