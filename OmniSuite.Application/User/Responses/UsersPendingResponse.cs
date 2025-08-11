namespace OmniSuite.Application.User.Responses
{
    public class UsersPendingResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public DateTime CreatedAt { get; set; }
        public UserStatusEnum Status { get; set; } = UserStatusEnum.registered;
        public decimal? FeePercentage { get; set; }
        public string? GatewayId { get; set; }
        public string? Role { get; set; }
    }
}
