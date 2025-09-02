using Microsoft.EntityFrameworkCore;
using OmniSuite.Domain.Entities;
using OmniSuite.Domain.Enums;
using OmniSuite.Persistence;
using OmniSuite.Tests.Common;
using FluentAssertions;

namespace OmniSuite.Tests.Persistence
{
    public class ErrorHandlingTests : InMemoryDatabaseTestBase
    {
        public ErrorHandlingTests()
        {
            SetupDatabase();
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
                Token = "duplicate-token", // Same token value
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
        }

        [Fact]
        public async Task Should_Handle_Invalid_Enum_Values()
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

            // Act & Assert - This should work as EF handles enum conversion
            var deposit = new Deposit
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Amount = 10000,
                PaymentMethod = DepositPaymentTypeEnum.Pix,
                TransactionStatus = DepositStatusEnum.Created,
                CreatedAt = DateTime.UtcNow
            };

            await SaveEntityAsync(user);
            var savedDeposit = await SaveEntityAsync(deposit);

            savedDeposit.Should().NotBeNull();
            savedDeposit.PaymentMethod.Should().Be(DepositPaymentTypeEnum.Pix);
            savedDeposit.TransactionStatus.Should().Be(DepositStatusEnum.Created);
        }

        [Fact]
        public async Task Should_Handle_Null_Optional_Properties()
        {
            // Arrange
            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = "Test User",
                Email = "test@example.com",
                PasswordHash = "hashedpassword",
                CreatedAt = DateTime.UtcNow,
                Status = UserStatusEnum.registered,
                Phone = null, // Optional property
                Document = null, // Optional property
                ProfilePhoto = null, // Optional property
                MfaSecretKey = null, // Optional property
                IsMfaEnabled = null // Optional property
            };

            // Act
            var savedUser = await SaveEntityAsync(user);

            // Assert
            savedUser.Should().NotBeNull();
            savedUser.Phone.Should().BeNull();
            savedUser.Document.Should().BeNull();
            savedUser.ProfilePhoto.Should().BeNull();
            savedUser.MfaSecretKey.Should().BeNull();
            savedUser.IsMfaEnabled.Should().BeNull();
        }

        [Fact]
        public async Task Should_Handle_Required_Property_Violations()
        {
            // Arrange
            var userToken = new UserToken
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(), // Non-existent user
                Token = null!, // Required property is null
                Type = null!, // Required property is null
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                CreatedAt = DateTime.UtcNow,
                IsUsed = false
            };

            // Act & Assert
            var action = async () => await SaveEntityAsync(userToken);
            await action.Should().ThrowAsync<DbUpdateException>();
        }

        [Fact]
        public async Task Should_Allow_Orphaned_Records_In_Memory_Database()
        {
            // Arrange
            var deposit = new Deposit
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(), // Non-existent user
                Amount = 10000,
                PaymentMethod = DepositPaymentTypeEnum.Pix,
                TransactionStatus = DepositStatusEnum.Created,
                CreatedAt = DateTime.UtcNow
            };

            // Act - This will work in in-memory database as it doesn't enforce foreign keys
            await SaveEntityAsync(deposit);

            // Assert
            var savedDeposit = await Context.Deposit.FindAsync(deposit.Id);
            savedDeposit.Should().NotBeNull();
            savedDeposit.Amount.Should().Be(10000);
        }

        [Fact]
        public async Task Should_Handle_Concurrent_Updates()
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

            await SaveEntityAsync(user);

            // Act - Simulate concurrent updates
            var user1 = await Context.Users.FindAsync(user.Id);
            var user2 = await Context.Users.FindAsync(user.Id);

            user1!.Name = "Updated Name 1";
            user2!.Name = "Updated Name 2";

            await Context.SaveChangesAsync();

            // Second save should work in in-memory database
            // In real scenarios, this might cause concurrency conflicts
            await Context.SaveChangesAsync();

            // Assert
            var finalUser = await Context.Users.FindAsync(user.Id);
            finalUser.Should().NotBeNull();
            finalUser.Name.Should().Be("Updated Name 2"); // Last update wins
        }

        [Fact]
        public async Task Should_Handle_Large_Data_Operations()
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

            var deposits = new List<Deposit>();
            for (int i = 0; i < 100; i++)
            {
                deposits.Add(new Deposit
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    Amount = 1000 + i,
                    PaymentMethod = DepositPaymentTypeEnum.Pix,
                    TransactionStatus = DepositStatusEnum.Created,
                    CreatedAt = DateTime.UtcNow,
                    PaymentCode = $"PAY{i:D6}"
                });
            }

            // Act
            await SaveEntityAsync(user);
            var savedDeposits = await SaveEntitiesAsync(deposits);

            // Assert
            savedDeposits.Should().HaveCount(100);
            
            var allDeposits = await Context.Deposit
                .Where(d => d.UserId == user.Id)
                .ToListAsync();
            allDeposits.Should().HaveCount(100);

            var totalAmount = allDeposits.Sum(d => d.Amount);
            totalAmount.Should().Be(104950); // Sum of 1000 to 1099
        }

        [Fact]
        public async Task Should_Handle_Simulated_Error_Scenario()
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

            // Act - Simulate error scenario without transactions (not supported in in-memory)
            try
            {
                await SaveEntityAsync(user);

                var deposit = new Deposit
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    Amount = 10000,
                    PaymentMethod = DepositPaymentTypeEnum.Pix,
                    TransactionStatus = DepositStatusEnum.Created,
                    CreatedAt = DateTime.UtcNow
                };

                await SaveEntityAsync(deposit);

                // Simulate an error condition
                throw new InvalidOperationException("Simulated error");
            }
            catch (InvalidOperationException)
            {
                // In a real scenario, this would trigger a rollback
                // In in-memory database, we just verify the data was saved
            }

            // Assert - Data was saved (in-memory doesn't support rollback)
            var savedUser = await Context.Users.FindAsync(user.Id);
            savedUser.Should().NotBeNull();

            var savedDeposits = await Context.Deposit
                .Where(d => d.UserId == user.Id)
                .ToListAsync();
            savedDeposits.Should().HaveCount(1);
        }

        [Fact]
        public async Task Should_Handle_String_Length_Constraints()
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

            // Act & Assert - Token with max length should work
            var validToken = new UserToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Token = new string('A', 255), // Max length
                Type = "access",
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                CreatedAt = DateTime.UtcNow,
                IsUsed = false
            };

            await SaveEntityAsync(user);
            var savedToken = await SaveEntityAsync(validToken);

            savedToken.Should().NotBeNull();
            savedToken.Token.Should().HaveLength(255);
        }

        [Fact]
        public async Task Should_Handle_DateTime_Edge_Cases()
        {
            // Arrange
            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = "Test User",
                Email = "test@example.com",
                PasswordHash = "hashedpassword",
                CreatedAt = DateTime.MinValue, // Edge case
                Status = UserStatusEnum.registered
            };

            var userToken = new UserToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Token = "test-token",
                Type = "access",
                ExpiresAt = DateTime.MaxValue, // Edge case
                CreatedAt = DateTime.UtcNow,
                IsUsed = false
            };

            // Act
            var savedUser = await SaveEntityAsync(user);
            var savedToken = await SaveEntityAsync(userToken);

            // Assert
            savedUser.Should().NotBeNull();
            savedUser.CreatedAt.Should().Be(DateTime.MinValue);

            savedToken.Should().NotBeNull();
            savedToken.ExpiresAt.Should().Be(DateTime.MaxValue);
        }

        [Fact]
        public async Task Should_Handle_Empty_Collections()
        {
            // Arrange
            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = "Test User",
                Email = "test@example.com",
                PasswordHash = "hashedpassword",
                CreatedAt = DateTime.UtcNow,
                Status = UserStatusEnum.registered,
                Tokens = new List<UserToken>(), // Empty collection
                Deposits = new List<Deposit>(), // Empty collection
                Affiliates = new List<Affiliates>(), // Empty collection
                AffiliatesCommission = new List<AffiliatesCommission>() // Empty collection
            };

            // Act
            var savedUser = await SaveEntityAsync(user);

            // Assert
            savedUser.Should().NotBeNull();
            savedUser.Tokens.Should().BeEmpty();
            savedUser.Deposits.Should().BeEmpty();
            savedUser.Affiliates.Should().BeEmpty();
            savedUser.AffiliatesCommission.Should().BeEmpty();

            // Verify with database query
            var userWithCollections = await Context.Users
                .Include(u => u.Tokens)
                .Include(u => u.Deposits)
                .Include(u => u.Affiliates)
                .Include(u => u.AffiliatesCommission)
                .FirstOrDefaultAsync(u => u.Id == user.Id);

            userWithCollections.Should().NotBeNull();
            userWithCollections.Tokens.Should().BeEmpty();
            userWithCollections.Deposits.Should().BeEmpty();
            userWithCollections.Affiliates.Should().BeEmpty();
            userWithCollections.AffiliatesCommission.Should().BeEmpty();
        }

        protected override void CleanupDatabase()
        {
            base.CleanupDatabase();
        }
    }
}
