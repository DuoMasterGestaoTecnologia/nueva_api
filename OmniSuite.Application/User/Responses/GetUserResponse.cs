namespace OmniSuite.Application.User.Responses
{
    public class GetUserResponse()
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; }
        public UserStatusEnum Status { get; set; }
        public decimal FeePercentage { get; set; }
        public string? DocumentNumber { get; set; }
        public string? ProfilePhoto { get; set; }
        public bool? IsMfaEnabled { get; set; }
        public string? Phone { get; set; }
        public long? Amount { get; set; }
    }   
}
