using FluentAssertions;
using OmniSuite.Domain.Entities;
using OmniSuite.Domain.Enums;
using Xunit;

namespace OmniSuite.Tests.Domain.Entities
{
    public class UserTests
    {
        [Fact]
        public void User_WhenCreated_ShouldHaveDefaultValues()
        {
            // Arrange & Act
            var user = new User();

            // Assert
            user.Should().NotBeNull();
            user.Id.Should().Be(Guid.Empty);
            user.Name.Should().BeNull();
            user.Email.Should().BeNull();
            user.PasswordHash.Should().BeNull();
            user.CreatedAt.Should().Be(DateTime.MinValue);
            user.Status.Should().Be(UserStatusEnum.registered);
            user.RefreshToken.Should().BeNull();
            user.RefreshTokenExpiresAt.Should().BeNull();
            user.MfaSecretKey.Should().BeNull();
            user.IsMfaEnabled.Should().BeNull();
            user.Phone.Should().BeNull();
            user.ProfilePhoto.Should().BeNull();
            user.Document.Should().BeNull();
            user.Deposits.Should().BeNull();
            user.Tokens.Should().NotBeNull();
            user.Tokens.Should().BeEmpty();
            user.Affiliates.Should().NotBeNull();
            user.Affiliates.Should().BeEmpty();
            user.AffiliatesCommission.Should().NotBeNull();
            user.AffiliatesCommission.Should().BeEmpty();
            user.UserBalance.Should().BeNull();
        }

        [Fact]
        public void User_WhenPropertiesSet_ShouldStoreValuesCorrectly()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var userName = "Test User";
            var userEmail = "test@example.com";
            var userPasswordHash = "hashed_password_123";
            var userCreatedAt = DateTime.UtcNow;
            var userStatus = UserStatusEnum.active;
            var userRefreshToken = "refresh_token_123";
            var userRefreshTokenExpiresAt = DateTime.UtcNow.AddDays(7);
            var userMfaSecretKey = "JBSWY3DPEHPK3PXP";
            var userIsMfaEnabled = true;
            var userPhone = "+5511999999999";
            var userProfilePhoto = "profile_photo_data";
            var userDocument = "12345678901";

            // Act
            var user = new User
            {
                Id = userId,
                Name = userName,
                Email = userEmail,
                PasswordHash = userPasswordHash,
                CreatedAt = userCreatedAt,
                Status = userStatus,
                RefreshToken = userRefreshToken,
                RefreshTokenExpiresAt = userRefreshTokenExpiresAt,
                MfaSecretKey = userMfaSecretKey,
                IsMfaEnabled = userIsMfaEnabled,
                Phone = userPhone,
                ProfilePhoto = userProfilePhoto,
                Document = userDocument
            };

            // Assert
            user.Id.Should().Be(userId);
            user.Name.Should().Be(userName);
            user.Email.Should().Be(userEmail);
            user.PasswordHash.Should().Be(userPasswordHash);
            user.CreatedAt.Should().Be(userCreatedAt);
            user.Status.Should().Be(userStatus);
            user.RefreshToken.Should().Be(userRefreshToken);
            user.RefreshTokenExpiresAt.Should().Be(userRefreshTokenExpiresAt);
            user.MfaSecretKey.Should().Be(userMfaSecretKey);
            user.IsMfaEnabled.Should().Be(userIsMfaEnabled);
            user.Phone.Should().Be(userPhone);
            user.ProfilePhoto.Should().Be(userProfilePhoto);
            user.Document.Should().Be(userDocument);
        }

        [Fact]
        public void User_WhenCollectionsInitialized_ShouldBeEmpty()
        {
            // Arrange & Act
            var user = new User();

            // Assert
            user.Tokens.Should().NotBeNull();
            user.Tokens.Should().BeEmpty();
            user.Affiliates.Should().NotBeNull();
            user.Affiliates.Should().BeEmpty();
            user.AffiliatesCommission.Should().NotBeNull();
            user.AffiliatesCommission.Should().BeEmpty();
        }

        [Fact]
        public void User_WhenCollectionsModified_ShouldReflectChanges()
        {
            // Arrange
            var user = new User();
            var token = new UserToken();
            var affiliate = new Affiliates();
            var affiliateCommission = new AffiliatesCommission();

            // Act
            user.Tokens.Add(token);
            user.Affiliates.Add(affiliate);
            user.AffiliatesCommission.Add(affiliateCommission);

            // Assert
            user.Tokens.Should().HaveCount(1);
            user.Tokens.Should().Contain(token);
            user.Affiliates.Should().HaveCount(1);
            user.Affiliates.Should().Contain(affiliate);
            user.AffiliatesCommission.Should().HaveCount(1);
            user.AffiliatesCommission.Should().Contain(affiliateCommission);
        }

        [Theory]
        [InlineData(UserStatusEnum.registered)]
        [InlineData(UserStatusEnum.active)]
        [InlineData(UserStatusEnum.inactive)]
        [InlineData(UserStatusEnum.suspended)]
        public void User_WhenStatusSet_ShouldAcceptAllValidStatuses(UserStatusEnum status)
        {
            // Arrange & Act
            var user = new User { Status = status };

            // Assert
            user.Status.Should().Be(status);
        }

        [Fact]
        public void User_WhenMfaEnabled_ShouldHaveSecretKey()
        {
            // Arrange
            var user = new User
            {
                MfaSecretKey = "JBSWY3DPEHPK3PXP",
                IsMfaEnabled = true
            };

            // Act & Assert
            user.IsMfaEnabled.Should().BeTrue();
            user.MfaSecretKey.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void User_WhenRefreshTokenSet_ShouldHaveExpirationDate()
        {
            // Arrange
            var user = new User
            {
                RefreshToken = "refresh_token_123",
                RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(7)
            };

            // Act & Assert
            user.RefreshToken.Should().NotBeNullOrEmpty();
            user.RefreshTokenExpiresAt.Should().BeAfter(DateTime.UtcNow);
        }

        [Fact]
        public void User_WhenProfilePhotoSet_ShouldStorePhotoData()
        {
            // Arrange
            var photoData = "data:image/jpeg;base64,/9j/4AAQSkZJRgABAQAAAQABAAD/2wBDAAYEBQYFBAYGBQYHBwYIChAKCgkJChQODwwQFxQYGBcUFhYaHSUfGhsjHBYWICwgIyYnKSopGR8tMC0oMCUoKSj/2wBDAQcHBwoIChMKChMoGhYaKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCj/wAARCAABAAEDASIAAhEBAxEB/8QAFQABAQAAAAAAAAAAAAAAAAAAAAv/xAAUEAEAAAAAAAAAAAAAAAAAAAAA/8QAFQEBAQAAAAAAAAAAAAAAAAAAAAX/xAAUEQEAAAAAAAAAAAAAAAAAAAAA/9oADAMBAAIRAxEAPwCdABmX/9k=";

            // Act
            var user = new User { ProfilePhoto = photoData };

            // Assert
            user.ProfilePhoto.Should().Be(photoData);
            user.ProfilePhoto.Should().Contain("data:image/jpeg;base64");
        }

        [Fact]
        public void User_WhenDocumentSet_ShouldStoreDocumentNumber()
        {
            // Arrange
            var documentNumber = "12345678901";

            // Act
            var user = new User { Document = documentNumber };

            // Assert
            user.Document.Should().Be(documentNumber);
            user.Document.Should().HaveLength(11);
        }

        [Fact]
        public void User_WhenPhoneSet_ShouldStorePhoneNumber()
        {
            // Arrange
            var phoneNumber = "+5511999999999";

            // Act
            var user = new User { Phone = phoneNumber };

            // Assert
            user.Phone.Should().Be(phoneNumber);
            user.Phone.Should().StartWith("+55");
        }
    }
}
