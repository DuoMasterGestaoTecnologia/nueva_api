using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using OmniSuite.Domain.Entities;
using OmniSuite.Domain.Enums;
using OmniSuite.Persistence;
using OmniSuite.Tests.Common;
using FluentAssertions;

namespace OmniSuite.Tests.Persistence
{
    public class EntityConfigurationTests : InMemoryDatabaseTestBase
    {
        public EntityConfigurationTests()
        {
            SetupDatabase();
        }

        [Fact]
        public void UserToken_Should_Have_Correct_Configuration()
        {
            // Arrange
            var entityType = Context.Model.FindEntityType(typeof(UserToken));

            // Assert
            entityType.Should().NotBeNull();

            // Check primary key
            var primaryKey = entityType.FindPrimaryKey();
            primaryKey.Should().NotBeNull();
            primaryKey.Properties.Should().HaveCount(1);
            primaryKey.Properties.First().Name.Should().Be("Id");

            // Check required properties
            var tokenProperty = entityType.FindProperty(nameof(UserToken.Token));
            tokenProperty.Should().NotBeNull();
            tokenProperty.IsNullable.Should().BeFalse();
            tokenProperty.GetMaxLength().Should().Be(255);

            var typeProperty = entityType.FindProperty(nameof(UserToken.Type));
            typeProperty.Should().NotBeNull();
            typeProperty.IsNullable.Should().BeFalse();
            typeProperty.GetMaxLength().Should().Be(50);

            var createdAtProperty = entityType.FindProperty(nameof(UserToken.CreatedAt));
            createdAtProperty.Should().NotBeNull();
            createdAtProperty.IsNullable.Should().BeFalse();

            var expiresAtProperty = entityType.FindProperty(nameof(UserToken.ExpiresAt));
            expiresAtProperty.Should().NotBeNull();
            expiresAtProperty.IsNullable.Should().BeFalse();

            var isUsedProperty = entityType.FindProperty(nameof(UserToken.IsUsed));
            isUsedProperty.Should().NotBeNull();
            isUsedProperty.IsNullable.Should().BeFalse();

            // Check unique index on Token
            var indexes = entityType.GetIndexes();
            var tokenIndex = indexes.FirstOrDefault(i => i.Properties.Any(p => p.Name == "Token"));
            tokenIndex.Should().NotBeNull();
            tokenIndex.IsUnique.Should().BeTrue();

            // Check foreign key relationship
            var foreignKeys = entityType.GetForeignKeys();
            var userForeignKey = foreignKeys.FirstOrDefault(fk => fk.PrincipalEntityType.ClrType == typeof(User));
            userForeignKey.Should().NotBeNull();
            userForeignKey.DeleteBehavior.Should().Be(DeleteBehavior.Cascade);
        }

        [Fact]
        public void Deposit_Should_Have_Correct_Enum_Conversions()
        {
            // Arrange
            var entityType = Context.Model.FindEntityType(typeof(Deposit));

            // Assert
            entityType.Should().NotBeNull();

            // Check enum properties exist
            var paymentMethodProperty = entityType.FindProperty(nameof(Deposit.PaymentMethod));
            paymentMethodProperty.Should().NotBeNull();
            paymentMethodProperty.ClrType.Should().Be(typeof(DepositPaymentTypeEnum));

            var transactionStatusProperty = entityType.FindProperty(nameof(Deposit.TransactionStatus));
            transactionStatusProperty.Should().NotBeNull();
            transactionStatusProperty.ClrType.Should().Be(typeof(DepositStatusEnum));
        }

        [Fact]
        public void Withdraw_Should_Have_Correct_Enum_Conversions()
        {
            // Arrange
            var entityType = Context.Model.FindEntityType(typeof(Withdraw));

            // Assert
            entityType.Should().NotBeNull();

            // Check enum property exists
            var transactionStatusProperty = entityType.FindProperty(nameof(Withdraw.TransactionStatus));
            transactionStatusProperty.Should().NotBeNull();
            transactionStatusProperty.ClrType.Should().Be(typeof(DepositStatusEnum));
        }

        [Fact]
        public void User_Should_Have_Correct_Table_Mapping()
        {
            // Arrange
            var entityType = Context.Model.FindEntityType(typeof(User));

            // Assert
            entityType.Should().NotBeNull();
            entityType.GetTableName().Should().Be("Users");
        }

        [Fact]
        public void UserToken_Should_Have_Correct_Table_Mapping()
        {
            // Arrange
            var entityType = Context.Model.FindEntityType(typeof(UserToken));

            // Assert
            entityType.Should().NotBeNull();
            entityType.GetTableName().Should().Be("UserTokens");
        }

        [Fact]
        public void Deposit_Should_Have_Correct_Table_Mapping()
        {
            // Arrange
            var entityType = Context.Model.FindEntityType(typeof(Deposit));

            // Assert
            entityType.Should().NotBeNull();
            entityType.GetTableName().Should().Be("Deposits");
        }

        [Fact]
        public void UserBalance_Should_Have_Correct_Table_Mapping()
        {
            // Arrange
            var entityType = Context.Model.FindEntityType(typeof(UserBalance));

            // Assert
            entityType.Should().NotBeNull();
            entityType.GetTableName().Should().Be("UserBalances");
        }

        [Fact]
        public void Withdraw_Should_Have_Correct_Table_Mapping()
        {
            // Arrange
            var entityType = Context.Model.FindEntityType(typeof(Withdraw));

            // Assert
            entityType.Should().NotBeNull();
            entityType.GetTableName().Should().Be("Withdraw");
        }

        [Fact]
        public void Affiliates_Should_Have_Correct_Table_Mapping()
        {
            // Arrange
            var entityType = Context.Model.FindEntityType(typeof(Affiliates));

            // Assert
            entityType.Should().NotBeNull();
            entityType.GetTableName().Should().Be("Affiliates");
        }

        [Fact]
        public void AffiliatesCommission_Should_Have_Correct_Table_Mapping()
        {
            // Arrange
            var entityType = Context.Model.FindEntityType(typeof(AffiliatesCommission));

            // Assert
            entityType.Should().NotBeNull();
            entityType.GetTableName().Should().Be("AffiliatesCommission");
        }

        [Fact]
        public void ActiveTransactions_Should_Have_Correct_Table_Mapping()
        {
            // Arrange
            var entityType = Context.Model.FindEntityType(typeof(ActiveTransactions));

            // Assert
            entityType.Should().NotBeNull();
            entityType.GetTableName().Should().Be("ActiveTransactions");
        }

        [Fact]
        public void User_Should_Have_Correct_Navigation_Properties()
        {
            // Arrange
            var entityType = Context.Model.FindEntityType(typeof(User));

            // Assert
            entityType.Should().NotBeNull();

            // Check navigation properties
            var tokensNavigation = entityType.FindNavigation(nameof(User.Tokens));
            tokensNavigation.Should().NotBeNull();
            tokensNavigation.IsCollection.Should().BeTrue();

            var depositsNavigation = entityType.FindNavigation(nameof(User.Deposits));
            depositsNavigation.Should().NotBeNull();
            depositsNavigation.IsCollection.Should().BeTrue();

            var affiliatesNavigation = entityType.FindNavigation(nameof(User.Affiliates));
            affiliatesNavigation.Should().NotBeNull();
            affiliatesNavigation.IsCollection.Should().BeTrue();

            var affiliatesCommissionNavigation = entityType.FindNavigation(nameof(User.AffiliatesCommission));
            affiliatesCommissionNavigation.Should().NotBeNull();
            affiliatesCommissionNavigation.IsCollection.Should().BeTrue();

            var userBalanceNavigation = entityType.FindNavigation(nameof(User.UserBalance));
            userBalanceNavigation.Should().NotBeNull();
            userBalanceNavigation.IsCollection.Should().BeFalse();
        }

        [Fact]
        public void UserToken_Should_Have_Correct_Navigation_Properties()
        {
            // Arrange
            var entityType = Context.Model.FindEntityType(typeof(UserToken));

            // Assert
            entityType.Should().NotBeNull();

            // Check navigation properties
            var userNavigation = entityType.FindNavigation(nameof(UserToken.User));
            userNavigation.Should().NotBeNull();
            userNavigation.IsCollection.Should().BeFalse();
        }

        [Fact]
        public void Deposit_Should_Have_Correct_Navigation_Properties()
        {
            // Arrange
            var entityType = Context.Model.FindEntityType(typeof(Deposit));

            // Assert
            entityType.Should().NotBeNull();

            // Check navigation properties
            var userNavigation = entityType.FindNavigation(nameof(Deposit.User));
            userNavigation.Should().NotBeNull();
            userNavigation.IsCollection.Should().BeFalse();
        }

        [Fact]
        public void Withdraw_Should_Have_Correct_Navigation_Properties()
        {
            // Arrange
            var entityType = Context.Model.FindEntityType(typeof(Withdraw));

            // Assert
            entityType.Should().NotBeNull();

            // Check navigation properties
            var userNavigation = entityType.FindNavigation(nameof(Withdraw.User));
            userNavigation.Should().NotBeNull();
            userNavigation.IsCollection.Should().BeFalse();
        }

        [Fact]
        public void Affiliates_Should_Have_Correct_Navigation_Properties()
        {
            // Arrange
            var entityType = Context.Model.FindEntityType(typeof(Affiliates));

            // Assert
            entityType.Should().NotBeNull();

            // Check navigation properties
            var userNavigation = entityType.FindNavigation(nameof(Affiliates.User));
            userNavigation.Should().NotBeNull();
            userNavigation.IsCollection.Should().BeFalse();
        }

        [Fact]
        public void AffiliatesCommission_Should_Have_Correct_Navigation_Properties()
        {
            // Arrange
            var entityType = Context.Model.FindEntityType(typeof(AffiliatesCommission));

            // Assert
            entityType.Should().NotBeNull();

            // Check navigation properties
            var affiliatesNavigation = entityType.FindNavigation(nameof(AffiliatesCommission.Affiliates));
            affiliatesNavigation.Should().NotBeNull();
            affiliatesNavigation.IsCollection.Should().BeFalse();

            var userNavigation = entityType.FindNavigation(nameof(AffiliatesCommission.User));
            userNavigation.Should().NotBeNull();
            userNavigation.IsCollection.Should().BeFalse();
        }

        [Fact]
        public void All_Entities_Should_Be_Registered_In_Model()
        {
            // Arrange & Act
            var entityTypes = Context.Model.GetEntityTypes();

            // Assert
            var entityTypeNames = entityTypes.Select(et => et.ClrType.Name).ToList();
            
            entityTypeNames.Should().Contain(nameof(User));
            entityTypeNames.Should().Contain(nameof(UserToken));
            entityTypeNames.Should().Contain(nameof(Deposit));
            entityTypeNames.Should().Contain(nameof(UserBalance));
            entityTypeNames.Should().Contain(nameof(Withdraw));
            entityTypeNames.Should().Contain(nameof(Affiliates));
            entityTypeNames.Should().Contain(nameof(AffiliatesCommission));
            entityTypeNames.Should().Contain(nameof(ActiveTransactions));
        }

        [Fact]
        public void DbContext_Should_Have_All_DbSets()
        {
            // Arrange & Act
            var dbSetProperties = typeof(ApplicationDbContext)
                .GetProperties()
                .Where(p => p.PropertyType.IsGenericType && 
                           p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
                .Select(p => p.Name)
                .ToList();

            // Assert
            dbSetProperties.Should().Contain(nameof(ApplicationDbContext.Users));
            dbSetProperties.Should().Contain(nameof(ApplicationDbContext.UserToken));
            dbSetProperties.Should().Contain(nameof(ApplicationDbContext.Deposit));
            dbSetProperties.Should().Contain(nameof(ApplicationDbContext.UserBalance));
            dbSetProperties.Should().Contain(nameof(ApplicationDbContext.Withdraw));
            dbSetProperties.Should().Contain(nameof(ApplicationDbContext.Affiliates));
            dbSetProperties.Should().Contain(nameof(ApplicationDbContext.AffiliatesCommission));
            dbSetProperties.Should().Contain(nameof(ApplicationDbContext.ActiveTransactions));
        }

        protected override void CleanupDatabase()
        {
            base.CleanupDatabase();
        }
    }
}
