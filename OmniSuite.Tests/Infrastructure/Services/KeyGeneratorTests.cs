using OmniSuite.Infrastructure.Services.KeyGenerator;
using Xunit;

namespace OmniSuite.Tests.Infrastructure.Services
{
    public class KeyGeneratorTests
    {
        [Fact]
        public void GeneratePublicKey_ShouldReturnValidBase64String()
        {
            // Act
            var publicKey = KeyGenerator.GeneratePublicKey();

            // Assert
            Assert.NotNull(publicKey);
            Assert.NotEmpty(publicKey);
            
            // Should be valid base64
            var bytes = Convert.FromBase64String(publicKey);
            Assert.Equal(32, bytes.Length); // 32 bytes = 256 bits
        }

        [Fact]
        public void GeneratePublicKey_ShouldGenerateDifferentKeys()
        {
            // Act
            var key1 = KeyGenerator.GeneratePublicKey();
            var key2 = KeyGenerator.GeneratePublicKey();

            // Assert
            Assert.NotEqual(key1, key2);
        }

        [Fact]
        public void GeneratePublicKey_ShouldGenerateRandomKeys()
        {
            // Act
            var keys = new List<string>();
            for (int i = 0; i < 100; i++)
            {
                keys.Add(KeyGenerator.GeneratePublicKey());
            }

            // Assert
            var uniqueKeys = keys.Distinct().Count();
            Assert.Equal(100, uniqueKeys); // All keys should be unique
        }

        [Fact]
        public void GeneratePrivateKey_ShouldReturnValidBase64String()
        {
            // Act
            var privateKey = KeyGenerator.GeneratePrivateKey();

            // Assert
            Assert.NotNull(privateKey);
            Assert.NotEmpty(privateKey);
            
            // Should be valid base64
            var bytes = Convert.FromBase64String(privateKey);
            Assert.Equal(64, bytes.Length); // 64 bytes = 512 bits
        }

        [Fact]
        public void GeneratePrivateKey_ShouldGenerateDifferentKeys()
        {
            // Act
            var key1 = KeyGenerator.GeneratePrivateKey();
            var key2 = KeyGenerator.GeneratePrivateKey();

            // Assert
            Assert.NotEqual(key1, key2);
        }

        [Fact]
        public void GeneratePrivateKey_ShouldGenerateRandomKeys()
        {
            // Act
            var keys = new List<string>();
            for (int i = 0; i < 100; i++)
            {
                keys.Add(KeyGenerator.GeneratePrivateKey());
            }

            // Assert
            var uniqueKeys = keys.Distinct().Count();
            Assert.Equal(100, uniqueKeys); // All keys should be unique
        }

        [Fact]
        public void GenerateHexKey_WithDefaultBytes_ShouldReturnValidHexString()
        {
            // Act
            var hexKey = KeyGenerator.GenerateHexKey();

            // Assert
            Assert.NotNull(hexKey);
            Assert.NotEmpty(hexKey);
            Assert.Equal(64, hexKey.Length); // 32 bytes * 2 = 64 hex characters
            
            // Should be valid hex
            Assert.True(hexKey.All(c => "0123456789abcdef".Contains(c)));
        }

        [Fact]
        public void GenerateHexKey_WithCustomBytes_ShouldReturnCorrectLength()
        {
            // Arrange
            var bytes = 16;

            // Act
            var hexKey = KeyGenerator.GenerateHexKey(bytes);

            // Assert
            Assert.NotNull(hexKey);
            Assert.NotEmpty(hexKey);
            Assert.Equal(32, hexKey.Length); // 16 bytes * 2 = 32 hex characters
            
            // Should be valid hex
            Assert.True(hexKey.All(c => "0123456789abcdef".Contains(c)));
        }

        [Fact]
        public void GenerateHexKey_WithZeroBytes_ShouldReturnEmptyString()
        {
            // Arrange
            var bytes = 0;

            // Act
            var hexKey = KeyGenerator.GenerateHexKey(bytes);

            // Assert
            Assert.NotNull(hexKey);
            Assert.Empty(hexKey);
        }

        [Fact]
        public void GenerateHexKey_ShouldGenerateDifferentKeys()
        {
            // Act
            var key1 = KeyGenerator.GenerateHexKey();
            var key2 = KeyGenerator.GenerateHexKey();

            // Assert
            Assert.NotEqual(key1, key2);
        }

        [Fact]
        public void GenerateHexKey_ShouldGenerateRandomKeys()
        {
            // Act
            var keys = new List<string>();
            for (int i = 0; i < 100; i++)
            {
                keys.Add(KeyGenerator.GenerateHexKey());
            }

            // Assert
            var uniqueKeys = keys.Distinct().Count();
            Assert.Equal(100, uniqueKeys); // All keys should be unique
        }

        [Fact]
        public void GenerateHexKey_WithNegativeBytes_ShouldThrowException()
        {
            // Arrange
            var bytes = -1;

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => KeyGenerator.GenerateHexKey(bytes));
        }

        [Fact]
        public void AllKeyGenerators_ShouldGenerateDifferentFormats()
        {
            // Act
            var publicKey = KeyGenerator.GeneratePublicKey();
            var privateKey = KeyGenerator.GeneratePrivateKey();
            var hexKey = KeyGenerator.GenerateHexKey();

            // Assert
            Assert.NotEqual(publicKey, privateKey);
            Assert.NotEqual(publicKey, hexKey);
            Assert.NotEqual(privateKey, hexKey);
            
            // Different formats should have different characteristics
            Assert.Contains("=", publicKey); // Base64 padding
            Assert.Contains("=", privateKey); // Base64 padding
            Assert.DoesNotContain("=", hexKey); // No padding in hex
        }
    }
}
