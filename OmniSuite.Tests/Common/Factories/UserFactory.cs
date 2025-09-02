using OmniSuite.Domain.Entities;
using OmniSuite.Domain.Enums;

namespace OmniSuite.Tests.Common.Factories
{
    public static class UserFactory
    {
        public static User CreateValidUser(
            Guid? id = null,
            string? name = null,
            string? email = null,
            string? passwordHash = null,
                         UserStatusEnum status = UserStatusEnum.approved)
        {
            return new User
            {
                Id = id ?? Guid.NewGuid(),
                Name = name ?? "Test User",
                Email = email ?? "test@example.com",
                PasswordHash = passwordHash ?? "hashed_password_123",
                CreatedAt = DateTime.UtcNow,
                Status = status,
                Phone = "+5511999999999",
                Document = "12345678901",
                ProfilePhoto = null,
                RefreshToken = null,
                RefreshTokenExpiresAt = null,
                MfaSecretKey = null,
                IsMfaEnabled = false,
                Deposits = new List<Deposit>(),
                Tokens = new List<UserToken>(),
                Affiliates = new List<Affiliates>(),
                AffiliatesCommission = new List<AffiliatesCommission>(),
                UserBalance = new UserBalance
                {
                    Id = Guid.NewGuid(),
                    UserId = id ?? Guid.NewGuid(),
                    TotalAmount = 1000L,
                    UpdatedAt = DateTime.UtcNow
                }
            };
        }

        public static User CreateInactiveUser()
        {
            return CreateValidUser(status: UserStatusEnum.inactive);
        }

        public static User CreateUserWithMFA(string mfaSecret = "JBSWY3DPEHPK3PXP")
        {
            var user = CreateValidUser();
            user.MfaSecretKey = mfaSecret;
            user.IsMfaEnabled = true;
            return user;
        }

        public static User CreateUserWithRefreshToken(string refreshToken = "valid_refresh_token")
        {
            var user = CreateValidUser();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(7);
            return user;
        }

        public static List<User> CreateMultipleUsers(int count = 3)
        {
            var users = new List<User>();
            for (int i = 0; i < count; i++)
            {
                users.Add(CreateValidUser(
                    name: $"Test User {i + 1}",
                    email: $"test{i + 1}@example.com"
                ));
            }
            return users;
        }
    }
}
