using Microsoft.EntityFrameworkCore;
using OmniSuite.Domain.Entities;
using OmniSuite.Domain.Enums;
using OmniSuite.Persistence;
using OmniSuite.Tests.Common;
using FluentAssertions;

namespace OmniSuite.Tests.Persistence
{
    public class CrudOperationsTests : InMemoryDatabaseTestBase
    {
        public CrudOperationsTests()
        {
            SetupDatabase();
        }

        [Fact]
        public async Task Should_Perform_Complex_User_Operations()
        {
            // Arrange
            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = "John Doe",
                Email = "john.doe@example.com",
                PasswordHash = "hashedpassword123",
                CreatedAt = DateTime.UtcNow,
                Status = UserStatusEnum.registered,
                Phone = "+1234567890",
                Document = "12345678901"
            };

            // Act - Create
            var createdUser = await SaveEntityAsync(user);

            // Assert - Create
            createdUser.Should().NotBeNull();
            createdUser.Name.Should().Be("John Doe");
            createdUser.Email.Should().Be("john.doe@example.com");
            createdUser.Status.Should().Be(UserStatusEnum.registered);

            // Act - Read
            var retrievedUser = await Context.Users
                .FirstOrDefaultAsync(u => u.Id == createdUser.Id);

            // Assert - Read
            retrievedUser.Should().NotBeNull();
            retrievedUser.Name.Should().Be("John Doe");
            retrievedUser.Phone.Should().Be("+1234567890");
            retrievedUser.Document.Should().Be("12345678901");

            // Act - Update
            retrievedUser.Name = "John Smith";
            retrievedUser.Status = UserStatusEnum.approved;
            retrievedUser.Phone = "+0987654321";
            await Context.SaveChangesAsync();

            // Assert - Update
            var updatedUser = await Context.Users
                .FirstOrDefaultAsync(u => u.Id == createdUser.Id);
            updatedUser.Name.Should().Be("John Smith");
            updatedUser.Status.Should().Be(UserStatusEnum.approved);
            updatedUser.Phone.Should().Be("+0987654321");

            // Act - Delete
            Context.Users.Remove(updatedUser);
            await Context.SaveChangesAsync();

            // Assert - Delete
            var deletedUser = await Context.Users
                .FirstOrDefaultAsync(u => u.Id == createdUser.Id);
            deletedUser.Should().BeNull();
        }

        [Fact]
        public async Task Should_Perform_Bulk_Operations_On_Deposits()
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

            var deposits = new List<Deposit>
            {
                new Deposit
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    Amount = 10000,
                    PaymentMethod = DepositPaymentTypeEnum.Pix,
                    TransactionStatus = DepositStatusEnum.Created,
                    CreatedAt = DateTime.UtcNow,
                    PaymentCode = "PAY001"
                },
                new Deposit
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    Amount = 25000,
                    PaymentMethod = DepositPaymentTypeEnum.CreditCard,
                    TransactionStatus = DepositStatusEnum.Payed,
                    CreatedAt = DateTime.UtcNow,
                    PaymentCode = "PAY002"
                },
                new Deposit
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    Amount = 5000,
                    PaymentMethod = DepositPaymentTypeEnum.BankSlip,
                    TransactionStatus = DepositStatusEnum.Error,
                    CreatedAt = DateTime.UtcNow,
                    PaymentCode = "PAY003"
                }
            };

            // Act
            await SaveEntityAsync(user);
            var savedDeposits = await SaveEntitiesAsync(deposits);

            // Assert
            savedDeposits.Should().HaveCount(3);
            
            var allDeposits = await Context.Deposit.ToListAsync();
            allDeposits.Should().HaveCount(3);
            
            var pendingDeposits = await Context.Deposit
                .Where(d => d.TransactionStatus == DepositStatusEnum.Created)
                .ToListAsync();
            pendingDeposits.Should().HaveCount(1);
            pendingDeposits.First().Amount.Should().Be(10000);

            var completedDeposits = await Context.Deposit
                .Where(d => d.TransactionStatus == DepositStatusEnum.Payed)
                .ToListAsync();
            completedDeposits.Should().HaveCount(1);
            completedDeposits.First().Amount.Should().Be(25000);
        }

        [Fact]
        public async Task Should_Handle_User_With_Multiple_Tokens()
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

            var tokens = new List<UserToken>
            {
                new UserToken
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    Token = "access-token-123",
                    Type = "access",
                    ExpiresAt = DateTime.UtcNow.AddHours(1),
                    CreatedAt = DateTime.UtcNow,
                    IsUsed = false
                },
                new UserToken
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    Token = "refresh-token-456",
                    Type = "refresh",
                    ExpiresAt = DateTime.UtcNow.AddDays(7),
                    CreatedAt = DateTime.UtcNow,
                    IsUsed = false
                },
                new UserToken
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    Token = "password-reset-789",
                    Type = "password_reset",
                    ExpiresAt = DateTime.UtcNow.AddHours(24),
                    CreatedAt = DateTime.UtcNow,
                    IsUsed = true,
                    UsedAt = DateTime.UtcNow.AddMinutes(5)
                }
            };

            // Act
            await SaveEntityAsync(user);
            await SaveEntitiesAsync(tokens);

            // Assert
            var userWithTokens = await Context.Users
                .Include(u => u.Tokens)
                .FirstOrDefaultAsync(u => u.Id == user.Id);

            userWithTokens.Should().NotBeNull();
            userWithTokens.Tokens.Should().HaveCount(3);

            var accessTokens = userWithTokens.Tokens.Where(t => t.Type == "access").ToList();
            accessTokens.Should().HaveCount(1);
            accessTokens.First().Token.Should().Be("access-token-123");

            var refreshTokens = userWithTokens.Tokens.Where(t => t.Type == "refresh").ToList();
            refreshTokens.Should().HaveCount(1);
            refreshTokens.First().Token.Should().Be("refresh-token-456");

            var usedTokens = userWithTokens.Tokens.Where(t => t.IsUsed).ToList();
            usedTokens.Should().HaveCount(1);
            usedTokens.First().Token.Should().Be("password-reset-789");
            usedTokens.First().UsedAt.Should().NotBeNull();
        }

        [Fact]
        public async Task Should_Handle_User_Balance_Operations()
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
                TotalAmount = 100000, // 1000.00
                TotalBlocked = 10000, // 100.00
                TotalPending = 5000,  // 50.00
                UpdatedAt = DateTime.UtcNow
            };

            // Act
            await SaveEntityAsync(user);
            await SaveEntityAsync(userBalance);

            // Assert
            var balance = await Context.UserBalance
                .FirstOrDefaultAsync(b => b.UserId == user.Id);

            balance.Should().NotBeNull();
            balance.TotalAmount.Should().Be(100000);
            balance.TotalBlocked.Should().Be(10000);
            balance.TotalPending.Should().Be(5000);

            // Act - Update balance
            balance.TotalAmount = 150000; // 1500.00
            balance.TotalBlocked = 20000; // 200.00
            balance.TotalPending = 0;
            balance.UpdatedAt = DateTime.UtcNow;
            await Context.SaveChangesAsync();

            // Assert - Updated balance
            var updatedBalance = await Context.UserBalance
                .FirstOrDefaultAsync(b => b.UserId == user.Id);

            updatedBalance.TotalAmount.Should().Be(150000);
            updatedBalance.TotalBlocked.Should().Be(20000);
            updatedBalance.TotalPending.Should().Be(0);
        }

        [Fact]
        public async Task Should_Handle_Affiliate_Commission_Chain()
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

            var commissions = new List<AffiliatesCommission>
            {
                new AffiliatesCommission
                {
                    Id = Guid.NewGuid(),
                    AffiliatesId = affiliate.Id,
                    UserId = user.Id,
                    Amount = 1000, // 10.00
                    CreatedAt = DateTime.UtcNow,
                    Type = TypeCommission.Deposit
                },
                new AffiliatesCommission
                {
                    Id = Guid.NewGuid(),
                    AffiliatesId = affiliate.Id,
                    UserId = user.Id,
                    Amount = 500, // 5.00
                    CreatedAt = DateTime.UtcNow.AddDays(1),
                    Type = TypeCommission.Trade
                }
            };

            // Act
            await SaveEntityAsync(user);
            await SaveEntityAsync(affiliate);
            await SaveEntitiesAsync(commissions);

            // Assert
            var affiliateWithCommissions = await Context.Affiliates
                .FirstOrDefaultAsync(a => a.Id == affiliate.Id);

            affiliateWithCommissions.Should().NotBeNull();
            affiliateWithCommissions.AffiliateCode.Should().Be("AFF123456");
            affiliateWithCommissions.CommissionPercent.Should().Be(5.5m);

            var totalCommission = await Context.AffiliatesCommission
                .Where(c => c.AffiliatesId == affiliate.Id)
                .SumAsync(c => c.Amount);

            totalCommission.Should().Be(1500); // 15.00

            var depositCommissions = await Context.AffiliatesCommission
                .Where(c => c.AffiliatesId == affiliate.Id && c.Type == TypeCommission.Deposit)
                .ToListAsync();

            depositCommissions.Should().HaveCount(1);
            depositCommissions.First().Amount.Should().Be(1000);
        }

        [Fact]
        public async Task Should_Handle_Active_Transactions_With_Time_Constraints()
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

            var now = DateTime.UtcNow;
            var activeTransactions = new List<ActiveTransactions>
            {
                new ActiveTransactions
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    EntranceTime = now,
                    ExpirationTime = now.AddMinutes(30),
                    CurrentChartValue = 50000, // 500.00
                    BuySellEnum = BuySellEnum.Buy,
                    InputValue = 100.00m,
                    OutputValue = 105.00m
                },
                new ActiveTransactions
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    EntranceTime = now.AddMinutes(5),
                    ExpirationTime = now.AddMinutes(35),
                    CurrentChartValue = 45000, // 450.00
                    BuySellEnum = BuySellEnum.Sell,
                    InputValue = 200.00m,
                    OutputValue = 195.00m
                }
            };

            // Act
            await SaveEntityAsync(user);
            await SaveEntitiesAsync(activeTransactions);

            // Assert
            var userTransactions = await Context.ActiveTransactions
                .Where(t => t.UserId == user.Id)
                .ToListAsync();

            userTransactions.Should().HaveCount(2);

            var buyTransactions = userTransactions.Where(t => t.BuySellEnum == BuySellEnum.Buy).ToList();
            buyTransactions.Should().HaveCount(1);
            buyTransactions.First().InputValue.Should().Be(100.00m);
            buyTransactions.First().OutputValue.Should().Be(105.00m);

            var sellTransactions = userTransactions.Where(t => t.BuySellEnum == BuySellEnum.Sell).ToList();
            sellTransactions.Should().HaveCount(1);
            sellTransactions.First().InputValue.Should().Be(200.00m);
            sellTransactions.First().OutputValue.Should().Be(195.00m);

            // Test time-based queries
            var expiredTransactions = await Context.ActiveTransactions
                .Where(t => t.UserId == user.Id && t.ExpirationTime < now.AddMinutes(32))
                .ToListAsync();

            expiredTransactions.Should().HaveCount(1);
            expiredTransactions.First().BuySellEnum.Should().Be(BuySellEnum.Buy);
        }

        [Fact]
        public async Task Should_Handle_Complex_Query_With_Multiple_Joins()
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
                Amount = 10000,
                PaymentMethod = DepositPaymentTypeEnum.Pix,
                TransactionStatus = DepositStatusEnum.Payed,
                CreatedAt = DateTime.UtcNow
            };

            var withdraw = new Withdraw
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Amount = 5000,
                TransactionStatus = DepositStatusEnum.Payed,
                CreatedAt = DateTime.UtcNow
            };

            var userBalance = new UserBalance
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                TotalAmount = 5000, // 50.00
                TotalBlocked = 0,
                TotalPending = 0,
                UpdatedAt = DateTime.UtcNow
            };

            // Act
            await SaveEntityAsync(user);
            await SaveEntityAsync(deposit);
            await SaveEntityAsync(withdraw);
            await SaveEntityAsync(userBalance);

            // Complex query with multiple joins
            var userSummary = await Context.Users
                .Where(u => u.Id == user.Id)
                .Select(u => new
                {
                    User = u,
                    TotalDeposits = u.Deposits.Sum(d => d.Amount),
                    TotalWithdraws = Context.Withdraw.Where(w => w.UserId == u.Id).Sum(w => w.Amount),
                    CurrentBalance = Context.UserBalance.Where(b => b.UserId == u.Id).Select(b => b.TotalAmount).FirstOrDefault(),
                    ActiveTokens = u.Tokens.Count(t => !t.IsUsed && t.ExpiresAt > DateTime.UtcNow)
                })
                .FirstOrDefaultAsync();

            // Assert
            userSummary.Should().NotBeNull();
            userSummary.User.Name.Should().Be("Test User");
            userSummary.TotalDeposits.Should().Be(10000);
            userSummary.TotalWithdraws.Should().Be(5000);
            userSummary.CurrentBalance.Should().Be(5000);
            userSummary.ActiveTokens.Should().Be(0); // No tokens created in this test
        }

        protected override void CleanupDatabase()
        {
            base.CleanupDatabase();
        }
    }
}
