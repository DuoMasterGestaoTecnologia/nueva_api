using OmniSuite.Application.Deposit.Responses;

namespace OmniSuite.Application.Deposit.Commands
{
    public class DepositCommand : IRequest<Response<DepositResponse>>
    {
        public decimal Amount { get; set; }

        public string Document { get; set; }
    }
}
