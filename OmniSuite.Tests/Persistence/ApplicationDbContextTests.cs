using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OmniSuite.Domain.Entities;
using OmniSuite.Domain.Enums;
using OmniSuite.Persistence;
using OmniSuite.Tests.Common;
using FluentAssertions;

namespace OmniSuite.Tests.Persistence
{
    public class ApplicationDbContextTests : InMemoryDatabaseTestBase
    {
        public ApplicationDbContextTests()
        {
            SetupDatabase();
        }

        [Fact]
        public async Task Should_Create_User_Entity_Successfully()
        {
            // Arrange
            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = "Test User",
                Email = "test@example.com",
                PasswordHash = "hashedpassword",
                CreatedAt = DateTime.UtcNow,
                Status = UserStatusEnum.registered
            };

            // Act
            var savedUser = await SaveEntityAsync(user);

            // Assert
            savedUser.Should().NotBeNull();
            savedUser.Id.Should().Be(user.Id);
            savedUser.Name.Should().Be(user.Name);
            savedUser.Email.Should().Be(user.Email);
            savedUser.Status.Should().Be(UserStatusEnum.registered);
        }

        [Fact]
        public async Task Should_Create_UserToken_Entity_With_User_Relationship()
        {
            // Arrange
            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = "Test User",
                Email = "test@example.com",
                PasswordHash = "hashedpassword",
                CreatedAt = DateTime.UtcNow,
                Status = UserStatusEnum.registered
            };

            var userToken = new UserToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Token = "test-token-123",
                Type = "access",
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                CreatedAt = DateTime.UtcNow,
                IsUsed = false
            };

            // Act
            await SaveEntityAsync(user);
            var savedToken = await SaveEntityAsync(userToken);

            // Assert
            savedToken.Should().NotBeNull();
            savedToken.UserId.Should().Be(user.Id);
            savedToken.Token.Should().Be("test-token-123");
            savedToken.Type.Should().Be("access");
            savedToken.IsUsed.Should().BeFalse();

            // Verify relationship
            var tokenWithUser = await Context.UserToken
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Id == savedToken.Id);

            tokenWithUser.Should().NotBeNull();
            tokenWithUser.User.Should().NotBeNull();
            tokenWithUser.User.Id.Should().Be(user.Id);
        }

        [Fact]
        public async Task Should_Allow_Duplicate_Tokens_In_Memory_Database()
        {
            // Arrange
            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = "Test User",
                Email = "test@example.com",
                PasswordHash = "hashedpassword",
                CreatedAt = DateTime.UtcNow,
                Status = UserStatusEnum.registered
            };

            var token1 = new UserToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Token = "duplicate-token",
                Type = "access",
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                CreatedAt = DateTime.UtcNow,
                IsUsed = false
            };

            var token2 = new UserToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Token = "duplicate-token", // Same token
                Type = "refresh",
                ExpiresAt = DateTime.UtcNow.AddHours(2),
                CreatedAt = DateTime.UtcNow,
                IsUsed = false
            };

            // Act
            await SaveEntityAsync(user);
            await SaveEntityAsync(token1);
            await SaveEntityAsync(token2); // This will work in in-memory database

            // Assert
            var tokens = await Context.UserToken
                .Where(t => t.Token == "duplicate-token")
                .ToListAsync();

            tokens.Should().HaveCount(2);
            tokens.Should().Contain(t => t.Type == "access");
            tokens.Should().Contain(t => t.Type == "refresh");
        }

        [Fact]
        public async Task Should_Create_Deposit_Entity_With_Enum_Conversions()
        {
            // Arrange
            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = "Test User",
                Email = "test@example.com",
                PasswordHash = "hashedpassword",
                CreatedAt = DateTime.UtcNow,
                Status = UserStatusEnum.registered
            };

            var deposit = new Deposit
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Amount = 10000, // 100.00 in cents
                PaymentMethod = DepositPaymentTypeEnum.Pix,
                TransactionStatus = DepositStatusEnum.Created,
                CreatedAt = DateTime.UtcNow,
                PaymentCode = "PAY123456",
                ExternalId = "EXT123456"
            };

            // Act
            await SaveEntityAsync(user);
            var savedDeposit = await SaveEntityAsync(deposit);

            // Assert
            savedDeposit.Should().NotBeNull();
            savedDeposit.Amount.Should().Be(10000);
            savedDeposit.PaymentMethod.Should().Be(DepositPaymentTypeEnum.Pix);
            savedDeposit.TransactionStatus.Should().Be(DepositStatusEnum.Created);
            savedDeposit.PaymentCode.Should().Be("PAY123456");
            savedDeposit.ExternalId.Should().Be("EXT123456");
        }

        [Fact]
        public async Task Should_Create_Withdraw_Entity_With_Enum_Conversions()
        {
            // Arrange
            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = "Test User",
                Email = "test@example.com",
                PasswordHash = "hashedpassword",
                CreatedAt = DateTime.UtcNow,
                Status = UserStatusEnum.registered
            };

            var withdraw = new Withdraw
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Amount = 5000, // 50.00 in cents
                TransactionStatus = DepositStatusEnum.Payed,
                CreatedAt = DateTime.UtcNow,
                ExternalId = "WIT123456"
            };

            // Act
            await SaveEntityAsync(user);
            var savedWithdraw = await SaveEntityAsync(withdraw);

            // Assert
            savedWithdraw.Should().NotBeNull();
            savedWithdraw.Amount.Should().Be(5000);
            savedWithdraw.TransactionStatus.Should().Be(DepositStatusEnum.Payed);
            savedWithdraw.ExternalId.Should().Be("WIT123456");
        }

        [Fact]
        public async Task Should_Create_UserBalance_Entity()
        {
            // Arrange
            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = "Test User",
                Email = "test@example.com",
                PasswordHash = "hashedpassword",
                CreatedAt = DateTime.UtcNow,
                Status = UserStatusEnum.registered
            };

            var userBalance = new UserBalance
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                TotalAmount = 100000, // 1000.00 in cents
                TotalBlocked = 10000, // 100.00 in cents
                TotalPending = 5000, // 50.00 in cents
                UpdatedAt = DateTime.UtcNow
            };

            // Act
            await SaveEntityAsync(user);
            var savedBalance = await SaveEntityAsync(userBalance);

            // Assert
            savedBalance.Should().NotBeNull();
            savedBalance.TotalAmount.Should().Be(100000);
            savedBalance.TotalBlocked.Should().Be(10000);
            savedBalance.TotalPending.Should().Be(5000);
        }

        [Fact]
        public async Task Should_Create_Affiliates_Entity()
        {
            // Arrange
            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = "Test User",
                Email = "test@example.com",
                PasswordHash = "hashedpassword",
                CreatedAt = DateTime.UtcNow,
                Status = UserStatusEnum.registered
            };

            var affiliate = new Affiliates
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                IsMarketUser = true,
                AffiliateCode = "AFF123456",
                CommissionPercent = 5.5m,
                TypeComission = TypeComission.Rave
            };

            // Act
            await SaveEntityAsync(user);
            var savedAffiliate = await SaveEntityAsync(affiliate);

            // Assert
            savedAffiliate.Should().NotBeNull();
            savedAffiliate.UserId.Should().Be(user.Id);
            savedAffiliate.IsMarketUser.Should().BeTrue();
            savedAffiliate.AffiliateCode.Should().Be("AFF123456");
            savedAffiliate.CommissionPercent.Should().Be(5.5m);
            savedAffiliate.TypeComission.Should().Be(TypeComission.Rave);
        }

        [Fact]
        public async Task Should_Create_AffiliatesCommission_Entity()
        {
            // Arrange
            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = "Test User",
                Email = "test@example.com",
                PasswordHash = "hashedpassword",
                CreatedAt = DateTime.UtcNow,
                Status = UserStatusEnum.registered
            };

            var affiliate = new Affiliates
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                IsMarketUser = true,
                AffiliateCode = "AFF123456",
                CommissionPercent = 5.5m,
                TypeComission = TypeComission.Rave
            };

            var commission = new AffiliatesCommission
            {
                Id = Guid.NewGuid(),
                AffiliatesId = affiliate.Id,
                UserId = user.Id,
                Amount = 1000, // 10.00 in cents
                CreatedAt = DateTime.UtcNow,
                Type = TypeCommission.Deposit
            };

            // Act
            await SaveEntityAsync(user);
            await SaveEntityAsync(affiliate);
            var savedCommission = await SaveEntityAsync(commission);

            // Assert
            savedCommission.Should().NotBeNull();
            savedCommission.AffiliatesId.Should().Be(affiliate.Id);
            savedCommission.UserId.Should().Be(user.Id);
            savedCommission.Amount.Should().Be(1000);
            savedCommission.Type.Should().Be(TypeCommission.Deposit);
        }

        [Fact]
        public async Task Should_Create_ActiveTransactions_Entity()
        {
            // Arrange
            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = "Test User",
                Email = "test@example.com",
                PasswordHash = "hashedpassword",
                CreatedAt = DateTime.UtcNow,
                Status = UserStatusEnum.registered
            };

            var activeTransaction = new ActiveTransactions
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                EntranceTime = DateTime.UtcNow,
                ExpirationTime = DateTime.UtcNow.AddMinutes(30),
                CurrentChartValue = 50000, // 500.00 in cents
                BuySellEnum = BuySellEnum.Buy,
                InputValue = 100.00m,
                OutputValue = 105.00m
            };

            // Act
            await SaveEntityAsync(user);
            var savedTransaction = await SaveEntityAsync(activeTransaction);

            // Assert
            savedTransaction.Should().NotBeNull();
            savedTransaction.UserId.Should().Be(user.Id);
            savedTransaction.CurrentChartValue.Should().Be(50000);
            savedTransaction.BuySellEnum.Should().Be(BuySellEnum.Buy);
            savedTransaction.InputValue.Should().Be(100.00m);
            savedTransaction.OutputValue.Should().Be(105.00m);
        }

        [Fact]
        public async Task Should_Handle_Cascade_Delete_For_UserTokens()
        {
            // Arrange
            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = "Test User",
                Email = "test@example.com",
                PasswordHash = "hashedpassword",
                CreatedAt = DateTime.UtcNow,
                Status = UserStatusEnum.registered
            };

            var userToken = new UserToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Token = "test-token-123",
                Type = "access",
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                CreatedAt = DateTime.UtcNow,
                IsUsed = false
            };

            // Act
            await SaveEntityAsync(user);
            await SaveEntityAsync(userToken);

            // Verify token exists
            var tokenExists = await Context.UserToken.AnyAsync(t => t.Id == userToken.Id);
            tokenExists.Should().BeTrue();

            // Delete user
            Context.Users.Remove(user);
            await Context.SaveChangesAsync();

            // Assert - token should be deleted due to cascade
            var tokenStillExists = await Context.UserToken.AnyAsync(t => t.Id == userToken.Id);
            tokenStillExists.Should().BeFalse();
        }

        [Fact]
        public async Task Should_Verify_Table_Mappings()
        {
            // Arrange & Act
            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = "Test User",
                Email = "test@example.com",
                PasswordHash = "hashedpassword",
                CreatedAt = DateTime.UtcNow,
                Status = UserStatusEnum.registered
            };

            await SaveEntityAsync(user);

            // Assert - Verify that entities are mapped to correct tables
            var entityType = Context.Model.FindEntityType(typeof(User));
            entityType.Should().NotBeNull();
            entityType.GetTableName().Should().Be("Users");

            var userTokenType = Context.Model.FindEntityType(typeof(UserToken));
            userTokenType.Should().NotBeNull();
            userTokenType.GetTableName().Should().Be("UserTokens");

            var depositType = Context.Model.FindEntityType(typeof(Deposit));
            depositType.Should().NotBeNull();
            depositType.GetTableName().Should().Be("Deposits");

            var withdrawType = Context.Model.FindEntityType(typeof(Withdraw));
            withdrawType.Should().NotBeNull();
            withdrawType.GetTableName().Should().Be("Withdraw");
        }

        [Fact]
        public async Task Should_Handle_Complex_Queries_With_Includes()
        {
            // Arrange
            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = "Test User",
                Email = "test@example.com",
                PasswordHash = "hashedpassword",
                CreatedAt = DateTime.UtcNow,
                Status = UserStatusEnum.registered
            };

            var userToken = new UserToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Token = "test-token-123",
                Type = "access",
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                CreatedAt = DateTime.UtcNow,
                IsUsed = false
            };

            var deposit = new Deposit
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Amount = 10000,
                PaymentMethod = DepositPaymentTypeEnum.Pix,
                TransactionStatus = DepositStatusEnum.Created,
                CreatedAt = DateTime.UtcNow
            };

            // Act
            await SaveEntityAsync(user);
            await SaveEntityAsync(userToken);
            await SaveEntityAsync(deposit);

            // Query with includes
            var userWithRelations = await Context.Users
                .Include(u => u.Tokens)
                .Include(u => u.Deposits)
                .FirstOrDefaultAsync(u => u.Id == user.Id);

            // Assert
            userWithRelations.Should().NotBeNull();
            userWithRelations.Tokens.Should().HaveCount(1);
            userWithRelations.Deposits.Should().HaveCount(1);
            userWithRelations.Tokens.First().Token.Should().Be("test-token-123");
            userWithRelations.Deposits.First().Amount.Should().Be(10000);
        }

        protected override void CleanupDatabase()
        {
            base.CleanupDatabase();
        }
    }
}
