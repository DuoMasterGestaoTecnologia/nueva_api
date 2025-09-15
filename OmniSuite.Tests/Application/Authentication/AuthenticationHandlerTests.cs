using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OmniSuite.Application.Authentication;
using OmniSuite.Application.Authentication.Commands;
using OmniSuite.Domain.Entities;
using OmniSuite.Domain.Interfaces;
using OmniSuite.Domain.Utils;
using OmniSuite.Persistence;
using OmniSuite.Tests.Common;
using OmniSuite.Tests.Common.Factories;
using Xunit;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

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

        private void SetupUserClaimsHelper(Guid userId)
        {
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var mockHttpContext = new Mock<HttpContext>();
            var mockClaimsPrincipal = new Mock<ClaimsPrincipal>();
            
            var userIdClaim = new Claim("userId", userId.ToString());
            mockClaimsPrincipal.Setup(x => x.FindFirst("userId")).Returns(userIdClaim);
            mockClaimsPrincipal.Setup(x => x.Identity!.IsAuthenticated).Returns(true);
            mockHttpContext.Setup(x => x.User).Returns(mockClaimsPrincipal.Object);
            mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(mockHttpContext.Object);
            
            UserClaimsHelper.Configure(mockHttpContextAccessor.Object);
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
            result.Success.Should().BeFalse();
            result.Data.Should().BeNull();
            result.Message.Should().NotBeNullOrEmpty();
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
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data!.Token.Should().Be(expectedAccessToken);
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
            result.Success.Should().BeFalse();
            result.Data.Should().BeNull();
            result.Message.Should().Be("Refresh token inválido ou expirado.");
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
            result.Success.Should().BeFalse();
            result.Data.Should().BeNull();
            result.Message.Should().Be("Refresh token inválido ou expirado.");
        }
    }
}
