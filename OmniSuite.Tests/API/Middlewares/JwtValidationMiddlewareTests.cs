using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Xunit;

namespace OmniSuite.Tests.API.Middlewares
{
    public class JwtValidationMiddlewareTests
    {
        private readonly Mock<RequestDelegate> _mockNext;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly JwtValidationMiddleware _middleware;
        private readonly DefaultHttpContext _context;

        public JwtValidationMiddlewareTests()
        {
            _mockNext = new Mock<RequestDelegate>();
            _mockConfiguration = new Mock<IConfiguration>();
            _middleware = new JwtValidationMiddleware(_mockNext.Object, _mockConfiguration.Object);
            _context = new DefaultHttpContext();

            // Setup configuration
            _mockConfiguration.Setup(x => x["Jwt:Key"]).Returns("test-secret-key-that-is-long-enough-for-hmac-sha256");
            _mockConfiguration.Setup(x => x["Jwt:Issuer"]).Returns("TestIssuer");
            _mockConfiguration.Setup(x => x["Jwt:Audience"]).Returns("TestAudience");
        }

        [Fact]
        public async Task Invoke_WithAllowAnonymousEndpoint_ShouldCallNext()
        {
            // Arrange
            var endpoint = new Endpoint(
                context => Task.CompletedTask,
                new EndpointMetadataCollection(new AllowAnonymousAttribute()),
                "test"
            );
            _context.SetEndpoint(endpoint);

            // Act
            await _middleware.Invoke(_context);

            // Assert
            _mockNext.Verify(x => x(_context), Times.Once);
        }

        [Fact]
        public async Task Invoke_WithValidToken_ShouldCallNext()
        {
            // Arrange
            var token = GenerateValidJwtToken();
            _context.Request.Headers["Authorization"] = $"Bearer {token}";

            // Act
            await _middleware.Invoke(_context);

            // Assert
            _mockNext.Verify(x => x(_context), Times.Once);
            Assert.True(_context.User.Identity?.IsAuthenticated);
        }

        [Fact]
        public async Task Invoke_WithInvalidToken_ShouldReturn401()
        {
            // Arrange
            _context.Request.Headers["Authorization"] = "Bearer invalid-token";

            // Act
            await _middleware.Invoke(_context);

            // Assert
            _mockNext.Verify(x => x(_context), Times.Never);
            Assert.Equal(401, _context.Response.StatusCode);
        }

        [Fact]
        public async Task Invoke_WithMalformedToken_ShouldReturn401()
        {
            // Arrange
            _context.Request.Headers["Authorization"] = "Bearer malformed.token";

            // Act
            await _middleware.Invoke(_context);

            // Assert
            _mockNext.Verify(x => x(_context), Times.Never);
            Assert.Equal(401, _context.Response.StatusCode);
        }

        [Fact]
        public async Task Invoke_WithExpiredToken_ShouldReturn401()
        {
            // Arrange
            var token = GenerateExpiredJwtToken();
            _context.Request.Headers["Authorization"] = $"Bearer {token}";

            // Act
            await _middleware.Invoke(_context);

            // Assert
            _mockNext.Verify(x => x(_context), Times.Never);
            Assert.Equal(401, _context.Response.StatusCode);
        }

        [Fact]
        public async Task Invoke_WithNoAuthorizationHeader_ShouldReturn401()
        {
            // Arrange
            // No authorization header set

            // Act
            await _middleware.Invoke(_context);

            // Assert
            _mockNext.Verify(x => x(_context), Times.Never);
            Assert.Equal(401, _context.Response.StatusCode);
        }

        [Fact]
        public async Task Invoke_WithEmptyAuthorizationHeader_ShouldReturn401()
        {
            // Arrange
            _context.Request.Headers["Authorization"] = "";

            // Act
            await _middleware.Invoke(_context);

            // Assert
            _mockNext.Verify(x => x(_context), Times.Never);
            Assert.Equal(401, _context.Response.StatusCode);
        }

        [Fact]
        public async Task Invoke_WithWrongTokenType_ShouldReturn401()
        {
            // Arrange
            _context.Request.Headers["Authorization"] = "Basic dGVzdDp0ZXN0";

            // Act
            await _middleware.Invoke(_context);

            // Assert
            _mockNext.Verify(x => x(_context), Times.Never);
            Assert.Equal(401, _context.Response.StatusCode);
        }

        [Fact]
        public async Task Invoke_WithValidToken_ShouldSetUserClaims()
        {
            // Arrange
            var token = GenerateValidJwtToken();
            _context.Request.Headers["Authorization"] = $"Bearer {token}";

            // Act
            await _middleware.Invoke(_context);

            // Assert
            Assert.True(_context.User.Identity?.IsAuthenticated);
            Assert.True(_context.User.HasClaim("userId", "test-user-id"));
            Assert.True(_context.User.HasClaim(ClaimTypes.NameIdentifier, "test-user-id"));
        }

        private string GenerateValidJwtToken()
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("test-secret-key-that-is-long-enough-for-hmac-sha256"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("userId", "test-user-id"),
                new Claim(JwtRegisteredClaimNames.Sub, "test-user-id"),
                new Claim(JwtRegisteredClaimNames.Email, "test@example.com"),
                new Claim(ClaimTypes.NameIdentifier, "test-user-id"),
                new Claim(ClaimTypes.Name, "Test User"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var token = new JwtSecurityToken(
                issuer: "TestIssuer",
                audience: "TestAudience",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateExpiredJwtToken()
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("test-secret-key-that-is-long-enough-for-hmac-sha256"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("userId", "test-user-id"),
                new Claim(JwtRegisteredClaimNames.Sub, "test-user-id"),
                new Claim(JwtRegisteredClaimNames.Email, "test@example.com"),
                new Claim(ClaimTypes.NameIdentifier, "test-user-id"),
                new Claim(ClaimTypes.Name, "Test User"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var token = new JwtSecurityToken(
                issuer: "TestIssuer",
                audience: "TestAudience",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(-1), // Expired
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
