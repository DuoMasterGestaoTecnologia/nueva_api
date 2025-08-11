namespace OmniSuite.Domain.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(Guid userId, string email, string Name);
        string GenerateRefreshToken();
    }
}
