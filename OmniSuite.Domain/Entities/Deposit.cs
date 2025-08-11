namespace OmniSuite.Domain.Entities
{
    public class Deposit
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public long Amount { get; set; }
        public DepositPaymentTypeEnum PaymentMethod { get; set; }
        public DepositStatusEnum TransactionStatus { get; set; }
        public DateTime CreatedAt { get; set; }

        public string? PaymentCode { get; set; }

        public string? ExternalId { get; set; }

        //references
        public User User { get; set; }

    }
}
