using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Moq;
using OmniSuite.Infrastructure.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Xunit;

namespace OmniSuite.Tests.Infrastructure.Services
{
    public class TokenServiceTests
    {
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly TokenService _tokenService;

        public TokenServiceTests()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            _tokenService = new TokenService(_mockConfiguration.Object);

            // Setup configuration
            _mockConfiguration.Setup(x => x["Jwt:Key"]).Returns("test-secret-key-that-is-long-enough-for-hmac-sha256");
            _mockConfiguration.Setup(x => x["Jwt:Issuer"]).Returns("TestIssuer");
            _mockConfiguration.Setup(x => x["Jwt:Audience"]).Returns("TestAudience");
        }

        [Fact]
        public void GenerateToken_WithValidParameters_ShouldReturnValidJwtToken()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var email = "test@example.com";
            var name = "Test User";

            // Act
            var token = _tokenService.GenerateToken(userId, email, name);

            // Assert
            Assert.NotNull(token);
            Assert.NotEmpty(token);

            // Verify token can be parsed
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            
            Assert.Equal(userId.ToString(), jwtToken.Claims.First(c => c.Type == "userId").Value);
            Assert.Equal(email, jwtToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.Email).Value);
            Assert.Equal(name, jwtToken.Claims.First(c => c.Type == "unique_name").Value);
            Assert.Equal("TestIssuer", jwtToken.Issuer);
            Assert.Contains("TestAudience", jwtToken.Audiences);
        }

        [Fact]
        public void GenerateToken_WithDifferentUsers_ShouldGenerateDifferentTokens()
        {
            // Arrange
            var userId1 = Guid.NewGuid();
            var userId2 = Guid.NewGuid();
            var email = "test@example.com";
            var name = "Test User";

            // Act
            var token1 = _tokenService.GenerateToken(userId1, email, name);
            var token2 = _tokenService.GenerateToken(userId2, email, name);

            // Assert
            Assert.NotEqual(token1, token2);
        }

        [Fact]
        public void GenerateToken_ShouldIncludeCorrectClaims()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var email = "test@example.com";
            var name = "Test User";

            // Act
            var token = _tokenService.GenerateToken(userId, email, name);

            // Assert
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            var claims = jwtToken.Claims.ToList();

            Assert.Contains(claims, c => c.Type == "userId" && c.Value == userId.ToString());
            Assert.Contains(claims, c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == userId.ToString());
            Assert.Contains(claims, c => c.Type == JwtRegisteredClaimNames.Email && c.Value == email);
            Assert.Contains(claims, c => c.Type == "nameid" && c.Value == userId.ToString());
            Assert.Contains(claims, c => c.Type == "unique_name" && c.Value == name);
            Assert.Contains(claims, c => c.Type == JwtRegisteredClaimNames.Jti);
        }

        [Fact]
        public void GenerateToken_ShouldHaveCorrectExpiration()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var email = "test@example.com";
            var name = "Test User";
            var beforeGeneration = DateTime.UtcNow;

            // Act
            var token = _tokenService.GenerateToken(userId, email, name);

            // Assert
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            
            var expectedExpiration = beforeGeneration.AddHours(24);
            Assert.True(jwtToken.ValidTo > expectedExpiration.AddMinutes(-1));
            Assert.True(jwtToken.ValidTo < expectedExpiration.AddMinutes(1));
        }

        [Fact]
        public void GenerateRefreshToken_ShouldReturnValidBase64String()
        {
            // Act
            var refreshToken = _tokenService.GenerateRefreshToken();

            // Assert
            Assert.NotNull(refreshToken);
            Assert.NotEmpty(refreshToken);
            
            // Should be valid base64
            var bytes = Convert.FromBase64String(refreshToken);
            Assert.Equal(32, bytes.Length); // 32 bytes = 256 bits
        }

        [Fact]
        public void GenerateRefreshToken_ShouldGenerateDifferentTokens()
        {
            // Act
            var token1 = _tokenService.GenerateRefreshToken();
            var token2 = _tokenService.GenerateRefreshToken();

            // Assert
            Assert.NotEqual(token1, token2);
        }

        [Fact]
        public void GenerateRefreshToken_ShouldGenerateRandomTokens()
        {
            // Act
            var tokens = new List<string>();
            for (int i = 0; i < 100; i++)
            {
                tokens.Add(_tokenService.GenerateRefreshToken());
            }

            // Assert
            var uniqueTokens = tokens.Distinct().Count();
            Assert.Equal(100, uniqueTokens); // All tokens should be unique
        }
    }
}
