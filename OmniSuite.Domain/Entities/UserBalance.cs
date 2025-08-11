namespace OmniSuite.Domain.Entities
{
    public class UserBalance
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public long TotalAmount { get; set; }
        public long? TotalBlocked { get; set; }
        public long? TotalPending { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
