namespace OmniSuite.Domain.Entities
{
    public class UserToken
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public string Token { get; set; } = default!;
        public string Type { get; set; } = default!;
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UsedAt { get; set; }
        public bool IsUsed { get; set; }

        public virtual User User { get; set; } = default!;
    }
}
