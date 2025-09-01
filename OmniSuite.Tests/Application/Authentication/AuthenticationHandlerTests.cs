using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OmniSuite.Application.Authentication;
using OmniSuite.Application.Authentication.Commands;
using OmniSuite.Application.Authentication.Responses;
using OmniSuite.Domain.Entities;
using OmniSuite.Domain.Interfaces;
using OmniSuite.Domain.Utils;
using OmniSuite.Persistence;
using OmniSuite.Tests.Common;
using OmniSuite.Tests.Common.Factories;
using Xunit;

namespace OmniSuite.Tests.Application.Authentication
{
    public class AuthenticationHandlerTests : InMemoryDatabaseTestBase
    {
        private readonly AuthenticationHandler _handler;
        private readonly Mock<ITokenService> _mockTokenService;
        private readonly Mock<IEmailService> _mockEmailService;

        public AuthenticationHandlerTests()
        {
            SetupDatabase();
            
            _mockTokenService = new Mock<ITokenService>();
            _mockEmailService = new Mock<IEmailService>();
            
            _handler = new AuthenticationHandler(_mockTokenService.Object, Context);
        }

        public override void Dispose()
        {
            CleanupDatabase();
            base.Dispose();
        }

        [Fact]
        public async Task Handle_LoginCommand_WithValidCredentials_ShouldReturnAuthenticationResponse()
        {
            // Arrange
            var user = UserFactory.CreateValidUser();
            await SaveEntityAsync(user);

            var command = CommandFactory.CreateValidLoginCommand(user.Email, "TestPassword123!");
            var expectedAccessToken = "valid_access_token";
            var expectedRefreshToken = "valid_refresh_token";

            _mockTokenService.Setup(x => x.GenerateToken(user.Id, user.Email, user.Name))
                           .Returns(expectedAccessToken);
            _mockTokenService.Setup(x => x.GenerateRefreshToken())
                           .Returns(expectedRefreshToken);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data!.AccessToken.Should().Be(expectedAccessToken);
            result.Data.RefreshToken.Should().Be(expectedRefreshToken);
            result.Data.Name.Should().Be(user.Name);
            result.Data.Email.Should().Be(user.Email);
            result.Data.ProfilePhoto.Should().Be(user.ProfilePhoto);
        }

        [Fact]
        public async Task Handle_LoginCommand_WithInvalidEmail_ShouldReturnFailure()
        {
            // Arrange
            var command = CommandFactory.CreateValidLoginCommand("nonexistent@example.com", "TestPassword123!");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeNull();
            result.ErrorMessage.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task Handle_RefreshTokenCommand_WithValidToken_ShouldReturnNewAuthenticationResponse()
        {
            // Arrange
            var user = UserFactory.CreateUserWithRefreshToken("valid_refresh_token");
            await SaveEntityAsync(user);

            var command = CommandFactory.CreateValidRefreshTokenCommand("valid_refresh_token");
            var expectedAccessToken = "new_access_token";
            var expectedRefreshToken = "new_refresh_token";

            _mockTokenService.Setup(x => x.GenerateToken(user.Id, user.Email, user.Name))
                           .Returns(expectedAccessToken);
            _mockTokenService.Setup(x => x.GenerateRefreshToken())
                           .Returns(expectedRefreshToken);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data!.AccessToken.Should().Be(expectedAccessToken);
            result.Data.RefreshToken.Should().Be(expectedRefreshToken);
        }

        [Fact]
        public async Task Handle_RefreshTokenCommand_WithInvalidToken_ShouldReturnFailure()
        {
            // Arrange
            var command = CommandFactory.CreateValidRefreshTokenCommand("invalid_refresh_token");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeNull();
            result.ErrorMessage.Should().Be("Refresh token inválido ou expirado.");
        }

        [Fact]
        public async Task Handle_RefreshTokenCommand_WithExpiredToken_ShouldReturnFailure()
        {
            // Arrange
            var user = UserFactory.CreateValidUser();
            user.RefreshToken = "expired_refresh_token";
            user.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(-1); // Expired
            await SaveEntityAsync(user);

            var command = CommandFactory.CreateValidRefreshTokenCommand("expired_refresh_token");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeNull();
            result.ErrorMessage.Should().Be("Refresh token inválido ou expirado.");
        }

        [Fact]
        public async Task Handle_LogoutCommand_ShouldClearRefreshToken()
        {
            // Arrange
            var user = UserFactory.CreateUserWithRefreshToken("valid_refresh_token");
            await SaveEntityAsync(user);

            var command = CommandFactory.CreateLogoutCommand();

            // Mock UserClaimsHelper to return the user ID
            var userId = user.Id;
            // Note: In a real test, you might need to mock the HttpContext or use a different approach

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().BeTrue();

            // Verify the refresh token was cleared in the database
            var updatedUser = await Context.Users.FindAsync(user.Id);
            updatedUser.Should().NotBeNull();
            updatedUser!.RefreshToken.Should().BeNull();
            updatedUser.RefreshTokenExpiresAt.Should().BeNull();
        }

        [Fact]
        public async Task Handle_LoginCommand_ShouldUpdateUserRefreshToken()
        {
            // Arrange
            var user = UserFactory.CreateValidUser();
            await SaveEntityAsync(user);

            var command = CommandFactory.CreateValidLoginCommand(user.Email, "TestPassword123!");
            var expectedRefreshToken = "new_refresh_token";

            _mockTokenService.Setup(x => x.GenerateToken(user.Id, user.Email, user.Name))
                           .Returns("access_token");
            _mockTokenService.Setup(x => x.GenerateRefreshToken())
                           .Returns(expectedRefreshToken);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            var updatedUser = await Context.Users.FindAsync(user.Id);
            updatedUser.Should().NotBeNull();
            updatedUser!.RefreshToken.Should().Be(expectedRefreshToken);
            updatedUser.RefreshTokenExpiresAt.Should().BeCloseTo(DateTime.UtcNow.AddDays(7), TimeSpan.FromMinutes(1));
        }
    }
}
