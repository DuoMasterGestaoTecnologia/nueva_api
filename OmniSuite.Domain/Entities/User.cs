namespace OmniSuite.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public DateTime CreatedAt { get; set; }
        public UserStatusEnum Status { get; set; } = UserStatusEnum.registered;
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiresAt { get; set; }      
        public string? MfaSecretKey { get; set; }
        public bool? IsMfaEnabled { get; set; }
        public string? Phone { get; set; }
        public string? ProfilePhoto { get; set; }
        public string? Document { get; set; }

        public ICollection<Deposit> Deposits { get; set; }
        public virtual ICollection<UserToken> Tokens { get; set; } = new List<UserToken>();

        public virtual ICollection<Affiliates> Affiliates { get; set; } = new List<Affiliates>();
        public virtual ICollection<AffiliatesCommission> AffiliatesCommission { get; set; } = new List<AffiliatesCommission>();

        public UserBalance UserBalance { get; set; }
    }
}
