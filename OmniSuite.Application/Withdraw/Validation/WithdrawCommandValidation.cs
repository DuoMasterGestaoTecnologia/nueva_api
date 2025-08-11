using Microsoft.AspNetCore.Http;
using OmniSuite.Application.Withdraw.Commands;
using OmniSuite.Domain.Utils;

namespace OmniSuite.Application.Withdraw.Validation
{
    public class WithdrawCommandValidation : AbstractValidator<WithdrawCommand>
    {
        private readonly ApplicationDbContext _context;

        public WithdrawCommandValidation(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;           

            RuleFor(x => x)
                .MustAsync(HaveEnoughBalance)
                .WithMessage("Saldo insuficiente para saque.");
        }

        private async Task<bool> HaveEnoughBalance(WithdrawCommand command, CancellationToken cancellationToken)
        {
            var userId = UserClaimsHelper.GetUserId();
            var userBalance = await _context.UserBalance.FirstOrDefaultAsync(x => x.UserId == userId);
            var requestedAmount = UtilsMoney.ToCents(command.Amount);

            if (userBalance is null)
            {
                return false;
            }

            return userBalance.TotalAmount >= requestedAmount;
        }
    }
}
