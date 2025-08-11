using OmniSuite.Application.Deposit.Responses;

namespace OmniSuite.Application.Withdraw.Commands
{
    public class WithdrawCommand : IRequest<Response<bool>>
    {
        public decimal Amount { get; set; }
        public string PixKey { get; set; }
        public string PixType { get; set; }
    }
}
