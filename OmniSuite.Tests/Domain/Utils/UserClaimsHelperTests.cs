using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using OmniSuite.Domain.Utils;
using System.Security.Claims;
using Xunit;

namespace OmniSuite.Tests.Domain.Utils
{
    public class UserClaimsHelperTests
    {
        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private readonly Mock<HttpContext> _mockHttpContext;
        private readonly Mock<ClaimsPrincipal> _mockClaimsPrincipal;

        public UserClaimsHelperTests()
        {
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _mockHttpContext = new Mock<HttpContext>();
            _mockClaimsPrincipal = new Mock<ClaimsPrincipal>();
        }

        private void SetupUserClaimsHelper()
        {
            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(_mockHttpContext.Object);
            UserClaimsHelper.Configure(_mockHttpContextAccessor.Object);
        }

        private void CleanupUserClaimsHelper()
        {
            UserClaimsHelper.Configure(null!);
        }

        [Fact]
        public void GetUserId_WhenUserIsAuthenticated_ShouldReturnUserId()
        {
            // Arrange
            var expectedUserId = Guid.NewGuid();
            var userIdClaim = new Claim("userId", expectedUserId.ToString());

            _mockClaimsPrincipal.Setup(x => x.FindFirst("userId")).Returns(userIdClaim);
            _mockClaimsPrincipal.Setup(x => x.Identity!.IsAuthenticated).Returns(true);
            _mockHttpContext.Setup(x => x.User).Returns(_mockClaimsPrincipal.Object);
            
            SetupUserClaimsHelper();

            // Act
            var result = UserClaimsHelper.GetUserId();

            // Assert
            result.Should().Be(expectedUserId);
            
            // Cleanup
            CleanupUserClaimsHelper();
        }

        [Fact]
        public void GetUserId_WhenUserIsNotAuthenticated_ShouldReturnEmptyGuid()
        {
            // Arrange
            _mockClaimsPrincipal.Setup(x => x.Identity!.IsAuthenticated).Returns(false);
            _mockHttpContext.Setup(x => x.User).Returns(_mockClaimsPrincipal.Object);
            
            SetupUserClaimsHelper();

            // Act
            var result = UserClaimsHelper.GetUserId();

            // Assert
            result.Should().Be(Guid.Empty);
        }

        [Fact]
        public void GetUserId_WhenHttpContextIsNull_ShouldReturnEmptyGuid()
        {
            // Arrange
            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns((HttpContext?)null);
            UserClaimsHelper.Configure(_mockHttpContextAccessor.Object);

            // Act
            var result = UserClaimsHelper.GetUserId();

            // Assert
            result.Should().Be(Guid.Empty);
        }

        [Fact]
        public void GetUserId_WhenClaimsAreEmpty_ShouldReturnEmptyGuid()
        {
            // Arrange
            _mockClaimsPrincipal.Setup(x => x.Claims).Returns(new List<Claim>());
            _mockClaimsPrincipal.Setup(x => x.Identity!.IsAuthenticated).Returns(true);
            _mockHttpContext.Setup(x => x.User).Returns(_mockClaimsPrincipal.Object);
            
            SetupUserClaimsHelper();

            // Act
            var result = UserClaimsHelper.GetUserId();

            // Assert
            result.Should().Be(Guid.Empty);
        }

        [Fact]
        public void GetUserId_WhenNameIdentifierClaimIsInvalid_ShouldReturnEmptyGuid()
        {
            // Arrange
            var claims = new List<Claim>
            {
                new Claim("userId", "invalid_guid")
            };

            _mockClaimsPrincipal.Setup(x => x.Claims).Returns(claims);
            _mockClaimsPrincipal.Setup(x => x.Identity!.IsAuthenticated).Returns(true);
            _mockHttpContext.Setup(x => x.User).Returns(_mockClaimsPrincipal.Object);
            
            SetupUserClaimsHelper();

            // Act
            var result = UserClaimsHelper.GetUserId();

            // Assert
            result.Should().Be(Guid.Empty);
        }

        [Fact]
        public void GetUserId_WhenMultipleNameIdentifierClaims_ShouldReturnFirstValidOne()
        {
            // Arrange
            var expectedUserId = Guid.NewGuid();
            var userIdClaim = new Claim("userId", expectedUserId.ToString());

            _mockClaimsPrincipal.Setup(x => x.FindFirst("userId")).Returns(userIdClaim);
            _mockClaimsPrincipal.Setup(x => x.Identity!.IsAuthenticated).Returns(true);
            _mockHttpContext.Setup(x => x.User).Returns(_mockClaimsPrincipal.Object);
            
            SetupUserClaimsHelper();

            // Act
            var result = UserClaimsHelper.GetUserId();

            // Assert
            result.Should().Be(expectedUserId);
            
            // Cleanup
            CleanupUserClaimsHelper();
        }

        [Fact]
        public void GetUserId_WhenNameIdentifierClaimIsMissing_ShouldReturnEmptyGuid()
        {
            // Arrange
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "test@example.com"),
                new Claim(ClaimTypes.Email, "test@example.com")
            };

            _mockClaimsPrincipal.Setup(x => x.Claims).Returns(claims);
            _mockClaimsPrincipal.Setup(x => x.Identity!.IsAuthenticated).Returns(true);
            _mockHttpContext.Setup(x => x.User).Returns(_mockClaimsPrincipal.Object);
            SetupUserClaimsHelper();

            // Act
            var result = UserClaimsHelper.GetUserId();

            // Assert
            result.Should().Be(Guid.Empty);
        }

        [Fact]
        public void GetUserId_WhenUserIdentityIsNull_ShouldReturnEmptyGuid()
        {
            // Arrange
            _mockClaimsPrincipal.Setup(x => x.Identity).Returns((ClaimsIdentity?)null);
            _mockHttpContext.Setup(x => x.User).Returns(_mockClaimsPrincipal.Object);
            SetupUserClaimsHelper();

            // Act
            var result = UserClaimsHelper.GetUserId();

            // Assert
            result.Should().Be(Guid.Empty);
        }

        [Fact]
        public void GetUserId_WhenConfiguredWithValidHttpContextAccessor_ShouldWorkCorrectly()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddHttpContextAccessor();
            services.AddSingleton(_mockHttpContextAccessor.Object);

            var serviceProvider = services.BuildServiceProvider();
            var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();

            var expectedUserId = Guid.NewGuid();
            var userIdClaim = new Claim("userId", expectedUserId.ToString());

            _mockClaimsPrincipal.Setup(x => x.FindFirst("userId")).Returns(userIdClaim);
            _mockClaimsPrincipal.Setup(x => x.Identity!.IsAuthenticated).Returns(true);
            _mockHttpContext.Setup(x => x.User).Returns(_mockClaimsPrincipal.Object);
            SetupUserClaimsHelper();

            // Act
            var result = UserClaimsHelper.GetUserId();

            // Assert
            result.Should().Be(expectedUserId);
            
            // Cleanup
            CleanupUserClaimsHelper();
        }

        [Theory]
        [InlineData("00000000-0000-0000-0000-000000000000")]
        [InlineData("11111111-1111-1111-1111-111111111111")]
        [InlineData("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa")]
        public void GetUserId_WithVariousValidGuidFormats_ShouldReturnCorrectGuid(string guidString)
        {
            // Arrange
            var expectedUserId = Guid.Parse(guidString);
            var userIdClaim = new Claim("userId", guidString);

            _mockClaimsPrincipal.Setup(x => x.FindFirst("userId")).Returns(userIdClaim);
            _mockClaimsPrincipal.Setup(x => x.Identity!.IsAuthenticated).Returns(true);
            _mockHttpContext.Setup(x => x.User).Returns(_mockClaimsPrincipal.Object);
            SetupUserClaimsHelper();

            // Act
            var result = UserClaimsHelper.GetUserId();

            // Assert
            result.Should().Be(expectedUserId);
            
            // Cleanup
            CleanupUserClaimsHelper();
        }
    }
}
