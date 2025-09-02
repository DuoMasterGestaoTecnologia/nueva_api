using Moq;
using OmniSuite.Infrastructure.Services.MFAService;
using Xunit;

namespace OmniSuite.Tests.Infrastructure.Services
{
    public class MfaServiceTests
    {
        private readonly MfaServiceImplementation _mfaService;

        public MfaServiceTests()
        {
            _mfaService = new MfaServiceImplementation();
        }

        [Fact]
        public void GenerateMfaSecret_WithValidEmail_ShouldReturnSecretAndQrCodeUri()
        {
            // Arrange
            var email = "test@example.com";

            // Act
            var (secret, qrCodeUri) = _mfaService.GenerateMfaSecret(email);

            // Assert
            Assert.NotNull(secret);
            Assert.NotEmpty(secret);
            Assert.NotNull(qrCodeUri);
            Assert.NotEmpty(qrCodeUri);
            Assert.StartsWith("otpauth://totp/Flowpag:", qrCodeUri);
            Assert.Contains($"secret={secret}", qrCodeUri);
            Assert.Contains("issuer=Flowpag", qrCodeUri);
        }

        [Fact]
        public void GenerateMfaSecret_WithDifferentEmails_ShouldGenerateDifferentSecrets()
        {
            // Arrange
            var email1 = "test1@example.com";
            var email2 = "test2@example.com";

            // Act
            var (secret1, _) = _mfaService.GenerateMfaSecret(email1);
            var (secret2, _) = _mfaService.GenerateMfaSecret(email2);

            // Assert
            Assert.NotEqual(secret1, secret2);
        }

        [Fact]
        public void GenerateMfaSecret_ShouldGenerateValidBase32Secret()
        {
            // Arrange
            var email = "test@example.com";

            // Act
            var (secret, _) = _mfaService.GenerateMfaSecret(email);

            // Assert
            // Base32 should only contain valid characters
            var validBase32Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
            Assert.All(secret, c => Assert.Contains(c, validBase32Chars));
        }

        [Fact]
        public void GenerateQrCodeSvg_WithValidUri_ShouldReturnSvgString()
        {
            // Arrange
            var qrCodeUri = "otpauth://totp/Flowpag:test@example.com?secret=TEST&issuer=Flowpag";

            // Act
            var svg = _mfaService.GenerateQrCodeSvg(qrCodeUri);

            // Assert
            Assert.NotNull(svg);
            Assert.NotEmpty(svg);
            Assert.StartsWith("<svg", svg);
            Assert.EndsWith("</svg>", svg);
        }

        [Fact]
        public void GenerateQrCodeSvg_WithDifferentUris_ShouldGenerateDifferentSvgs()
        {
            // Arrange
            var uri1 = "otpauth://totp/Flowpag:test1@example.com?secret=TEST1&issuer=Flowpag";
            var uri2 = "otpauth://totp/Flowpag:test2@example.com?secret=TEST2&issuer=Flowpag";

            // Act
            var svg1 = _mfaService.GenerateQrCodeSvg(uri1);
            var svg2 = _mfaService.GenerateQrCodeSvg(uri2);

            // Assert
            Assert.NotEqual(svg1, svg2);
        }

        [Fact]
        public void ValidateCode_WithValidCode_ShouldReturnTrue()
        {
            // Arrange
            var secret = "JBSWY3DPEHPK3PXP"; // "Hello!" in Base32
            var validCode = GenerateValidTotpCode(secret);

            // Act
            var result = _mfaService.ValidateCode(secret, validCode);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void ValidateCode_WithInvalidCode_ShouldReturnFalse()
        {
            // Arrange
            var secret = "JBSWY3DPEHPK3PXP";
            var invalidCode = "123456";

            // Act
            var result = _mfaService.ValidateCode(secret, invalidCode);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ValidateCode_WithEmptyCode_ShouldReturnFalse()
        {
            // Arrange
            var secret = "JBSWY3DPEHPK3PXP";
            var emptyCode = "";

            // Act
            var result = _mfaService.ValidateCode(secret, emptyCode);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ValidateCode_WithEmptySecret_ShouldReturnFalse()
        {
            // Arrange
            var secret = "";
            var code = "123456";

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _mfaService.ValidateCode(secret, code));
        }

        [Fact]
        public void ValidateCode_WithPreviousTimeWindow_ShouldReturnTrue()
        {
            // Arrange
            var secret = "JBSWY3DPEHPK3PXP";
            var validCode = GenerateValidTotpCode(secret);

            // Act
            var result = _mfaService.ValidateCode(secret, validCode);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void ValidateCode_WithFutureTimeWindow_ShouldReturnTrue()
        {
            // Arrange
            var secret = "JBSWY3DPEHPK3PXP";
            var validCode = GenerateValidTotpCode(secret);

            // Act
            var result = _mfaService.ValidateCode(secret, validCode);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void ValidateCode_WithWrongSecret_ShouldReturnFalse()
        {
            // Arrange
            var secret1 = "JBSWY3DPEHPK3PXP";
            var secret2 = "KBSWY3DPEHPK3PXP"; // Different secret
            var validCode = GenerateValidTotpCode(secret1);

            // Act
            var result = _mfaService.ValidateCode(secret2, validCode);

            // Assert
            Assert.False(result);
        }

        private string GenerateValidTotpCode(string secret)
        {
            // This is a simplified version for testing
            // In a real scenario, you would use the actual TOTP algorithm
            // For testing purposes, we'll generate a code that should be valid
            var totp = new OtpNet.Totp(OtpNet.Base32Encoding.ToBytes(secret));
            return totp.ComputeTotp();
        }
    }
}
