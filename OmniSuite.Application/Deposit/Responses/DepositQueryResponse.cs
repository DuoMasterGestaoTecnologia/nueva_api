namespace OmniSuite.Application.Deposit.Responses
{
    public class DepositQueryResponse
    {
        public Guid Id { get; set; }       
        public long Amount { get; set; }
        public DepositPaymentTypeEnum PaymentMethod { get; set; }
        public DepositStatusEnum TransactionStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? PaymentCode { get; set; }
    }
}
