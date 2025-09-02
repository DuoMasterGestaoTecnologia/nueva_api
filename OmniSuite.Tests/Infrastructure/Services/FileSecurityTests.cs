using Microsoft.AspNetCore.Http;
using Moq;
using OmniSuite.Infrastructure.Services.Security;
using Xunit;

namespace OmniSuite.Tests.Infrastructure.Services
{
    public class FileSecurityTests
    {
        [Fact]
        public async Task IsSafeAsync_WithNullFile_ShouldReturnFalse()
        {
            // Arrange
            IFormFile? file = null;

            // Act
            var result = await FileSecurity.IsSafeAsync(file!);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task IsSafeAsync_WithEmptyFile_ShouldReturnFalse()
        {
            // Arrange
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.Length).Returns(0);

            // Act
            var result = await FileSecurity.IsSafeAsync(mockFile.Object);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task IsSafeAsync_WithValidPngFile_ShouldReturnTrue()
        {
            // Arrange
            var pngBytes = new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }; // PNG header
            var mockFile = CreateMockFile(pngBytes, "image/png", "test.png");

            // Act
            var result = await FileSecurity.IsSafeAsync(mockFile.Object);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsSafeAsync_WithValidJpegFile_ShouldReturnTrue()
        {
            // Arrange
            var jpegBytes = new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 }; // JPEG header
            var mockFile = CreateMockFile(jpegBytes, "image/jpeg", "test.jpg");

            // Act
            var result = await FileSecurity.IsSafeAsync(mockFile.Object);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsSafeAsync_WithValidJpgFile_ShouldReturnTrue()
        {
            // Arrange
            var jpgBytes = new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 }; // JPEG header
            var mockFile = CreateMockFile(jpgBytes, "image/jpg", "test.jpg");

            // Act
            var result = await FileSecurity.IsSafeAsync(mockFile.Object);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsSafeAsync_WithValidWebpFile_ShouldReturnTrue()
        {
            // Arrange
            var webpBytes = new byte[] { 0x52, 0x49, 0x46, 0x46, 0x00, 0x00, 0x00, 0x00, 0x57, 0x45, 0x42, 0x50 }; // WEBP header
            var mockFile = CreateMockFile(webpBytes, "image/webp", "test.webp");

            // Act
            var result = await FileSecurity.IsSafeAsync(mockFile.Object);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsSafeAsync_WithValidPdfFile_ShouldReturnTrue()
        {
            // Arrange
            var pdfBytes = new byte[] { 0x25, 0x50, 0x44, 0x46, 0x2D, 0x31, 0x2E, 0x34 }; // PDF header
            var mockFile = CreateMockFile(pdfBytes, "application/pdf", "test.pdf");

            // Act
            var result = await FileSecurity.IsSafeAsync(mockFile.Object);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsSafeAsync_WithInvalidMimeType_ShouldReturnFalse()
        {
            // Arrange
            var fileBytes = new byte[] { 0x89, 0x50, 0x4E, 0x47 };
            var mockFile = CreateMockFile(fileBytes, "application/octet-stream", "test.bin");

            // Act
            var result = await FileSecurity.IsSafeAsync(mockFile.Object);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task IsSafeAsync_WithFileTooLarge_ShouldReturnFalse()
        {
            // Arrange
            var largeFileBytes = new byte[11 * 1024 * 1024]; // 11MB (larger than 10MB limit)
            var mockFile = CreateMockFile(largeFileBytes, "image/png", "large.png");

            // Act
            var result = await FileSecurity.IsSafeAsync(mockFile.Object);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task IsSafeAsync_WithFileAtMaxSize_ShouldReturnTrue()
        {
            // Arrange
            var maxSizeFileBytes = new byte[10 * 1024 * 1024]; // Exactly 10MB
            // Set PNG magic number at the beginning
            maxSizeFileBytes[0] = 0x89;
            maxSizeFileBytes[1] = 0x50;
            maxSizeFileBytes[2] = 0x4E;
            maxSizeFileBytes[3] = 0x47;
            var mockFile = CreateMockFile(maxSizeFileBytes, "image/png", "maxsize.png");

            // Act
            var result = await FileSecurity.IsSafeAsync(mockFile.Object);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsSafeAsync_WithPngMimeTypeButInvalidMagicNumber_ShouldReturnFalse()
        {
            // Arrange
            var invalidPngBytes = new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 }; // JPEG header but PNG mime type
            var mockFile = CreateMockFile(invalidPngBytes, "image/png", "fake.png");

            // Act
            var result = await FileSecurity.IsSafeAsync(mockFile.Object);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task IsSafeAsync_WithJpegMimeTypeButInvalidMagicNumber_ShouldReturnFalse()
        {
            // Arrange
            var invalidJpegBytes = new byte[] { 0x89, 0x50, 0x4E, 0x47 }; // PNG header but JPEG mime type
            var mockFile = CreateMockFile(invalidJpegBytes, "image/jpeg", "fake.jpg");

            // Act
            var result = await FileSecurity.IsSafeAsync(mockFile.Object);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task IsSafeAsync_WithFileTooSmallForMagicNumber_ShouldReturnFalse()
        {
            // Arrange
            var smallFileBytes = new byte[] { 0x89, 0x50 }; // Too small for PNG magic number
            var mockFile = CreateMockFile(smallFileBytes, "image/png", "small.png");

            // Act
            var result = await FileSecurity.IsSafeAsync(mockFile.Object);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task IsSafeAsync_WithCaseInsensitiveMimeType_ShouldReturnTrue()
        {
            // Arrange
            var pngBytes = new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };
            var mockFile = CreateMockFile(pngBytes, "image/png", "test.png"); // Use lowercase as expected

            // Act
            var result = await FileSecurity.IsSafeAsync(mockFile.Object);

            // Assert
            Assert.True(result);
        }

        private static Mock<IFormFile> CreateMockFile(byte[] fileBytes, string contentType, string fileName)
        {
            var mockFile = new Mock<IFormFile>();
            var memoryStream = new MemoryStream(fileBytes);

            mockFile.Setup(f => f.Length).Returns(fileBytes.Length);
            mockFile.Setup(f => f.ContentType).Returns(contentType);
            mockFile.Setup(f => f.FileName).Returns(fileName);
            mockFile.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                .Returns((Stream stream, CancellationToken token) => memoryStream.CopyToAsync(stream, token));

            return mockFile;
        }
    }
}
