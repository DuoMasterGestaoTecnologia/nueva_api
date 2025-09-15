using OmniSuite.Domain.Entities;
using OmniSuite.Domain.Enums;
using Xunit;

namespace OmniSuite.Tests.Domain
{
    public class DigitalProductTests
    {
        [Fact]
        public void DigitalProduct_Should_Initialize_With_Default_Values()
        {
            // Act
            var product = new DigitalProduct();

            // Assert
            Assert.Equal(DigitalProductStatusEnum.Active, product.Status);
            Assert.False(product.IsFeatured); // Default value is false
            Assert.True(product.IsDigitalDelivery);
            Assert.NotNull(product.Purchases);
            Assert.Empty(product.Purchases);
        }

        [Fact]
        public void DigitalProduct_Should_Set_Properties_Correctly()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var now = DateTime.UtcNow;

            // Act
            var product = new DigitalProduct
            {
                Id = productId,
                Name = "Test E-book",
                Description = "A comprehensive test e-book",
                ShortDescription = "Short description",
                Price = 29.99m,
                ImageUrl = "https://example.com/image.jpg",
                ThumbnailUrl = "https://example.com/thumbnail.jpg",
                Type = DigitalProductTypeEnum.Ebook,
                Status = DigitalProductStatusEnum.Active,
                DownloadUrl = "https://example.com/download",
                AccessInstructions = "Download and enjoy!",
                DownloadLimit = 5,
                ExpirationDate = now.AddDays(30),
                IsFeatured = true,
                IsDigitalDelivery = true,
                CategoryId = Guid.NewGuid(),
                Tags = "ebook, education, learning",
                CreatedAt = now,
                UpdatedAt = now,
                CreatedBy = userId
            };

            // Assert
            Assert.Equal(productId, product.Id);
            Assert.Equal("Test E-book", product.Name);
            Assert.Equal("A comprehensive test e-book", product.Description);
            Assert.Equal("Short description", product.ShortDescription);
            Assert.Equal(29.99m, product.Price);
            Assert.Equal("https://example.com/image.jpg", product.ImageUrl);
            Assert.Equal("https://example.com/thumbnail.jpg", product.ThumbnailUrl);
            Assert.Equal(DigitalProductTypeEnum.Ebook, product.Type);
            Assert.Equal(DigitalProductStatusEnum.Active, product.Status);
            Assert.Equal("https://example.com/download", product.DownloadUrl);
            Assert.Equal("Download and enjoy!", product.AccessInstructions);
            Assert.Equal(5, product.DownloadLimit);
            Assert.Equal(now.AddDays(30), product.ExpirationDate);
            Assert.True(product.IsFeatured);
            Assert.True(product.IsDigitalDelivery);
            Assert.NotNull(product.CategoryId);
            Assert.Equal("ebook, education, learning", product.Tags);
            Assert.Equal(now, product.CreatedAt);
            Assert.Equal(now, product.UpdatedAt);
            Assert.Equal(userId, product.CreatedBy);
        }

        [Theory]
        [InlineData(DigitalProductTypeEnum.Ebook)]
        [InlineData(DigitalProductTypeEnum.OnlineCourse)]
        [InlineData(DigitalProductTypeEnum.Software)]
        [InlineData(DigitalProductTypeEnum.Template)]
        [InlineData(DigitalProductTypeEnum.Plugin)]
        [InlineData(DigitalProductTypeEnum.Subscription)]
        [InlineData(DigitalProductTypeEnum.Other)]
        public void DigitalProduct_Should_Accept_All_Valid_Types(DigitalProductTypeEnum type)
        {
            // Arrange & Act
            var product = new DigitalProduct
            {
                Type = type
            };

            // Assert
            Assert.Equal(type, product.Type);
        }

        [Theory]
        [InlineData(DigitalProductStatusEnum.Active)]
        [InlineData(DigitalProductStatusEnum.Inactive)]
        [InlineData(DigitalProductStatusEnum.Draft)]
        [InlineData(DigitalProductStatusEnum.Archived)]
        public void DigitalProduct_Should_Accept_All_Valid_Statuses(DigitalProductStatusEnum status)
        {
            // Arrange & Act
            var product = new DigitalProduct
            {
                Status = status
            };

            // Assert
            Assert.Equal(status, product.Status);
        }
    }

    public class DigitalProductPurchaseTests
    {
        [Fact]
        public void DigitalProductPurchase_Should_Initialize_With_Default_Values()
        {
            // Act
            var purchase = new DigitalProductPurchase();

            // Assert
            Assert.Equal(0, purchase.DownloadCount);
        }

        [Fact]
        public void DigitalProductPurchase_Should_Set_Properties_Correctly()
        {
            // Arrange
            var purchaseId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var now = DateTime.UtcNow;

            // Act
            var purchase = new DigitalProductPurchase
            {
                Id = purchaseId,
                UserId = userId,
                DigitalProductId = productId,
                Amount = 29.99m,
                PurchaseDate = now,
                Status = DigitalProductPurchaseStatusEnum.Paid,
                DownloadToken = "test-download-token",
                DownloadTokenExpiresAt = now.AddDays(30),
                DownloadCount = 2
            };

            // Assert
            Assert.Equal(purchaseId, purchase.Id);
            Assert.Equal(userId, purchase.UserId);
            Assert.Equal(productId, purchase.DigitalProductId);
            Assert.Equal(29.99m, purchase.Amount);
            Assert.Equal(now, purchase.PurchaseDate);
            Assert.Equal(DigitalProductPurchaseStatusEnum.Paid, purchase.Status);
            Assert.Equal("test-download-token", purchase.DownloadToken);
            Assert.Equal(now.AddDays(30), purchase.DownloadTokenExpiresAt);
            Assert.Equal(2, purchase.DownloadCount);
        }

        [Theory]
        [InlineData(DigitalProductPurchaseStatusEnum.Pending)]
        [InlineData(DigitalProductPurchaseStatusEnum.Paid)]
        [InlineData(DigitalProductPurchaseStatusEnum.Delivered)]
        [InlineData(DigitalProductPurchaseStatusEnum.Cancelled)]
        [InlineData(DigitalProductPurchaseStatusEnum.Refunded)]
        public void DigitalProductPurchase_Should_Accept_All_Valid_Statuses(DigitalProductPurchaseStatusEnum status)
        {
            // Arrange & Act
            var purchase = new DigitalProductPurchase
            {
                Status = status
            };

            // Assert
            Assert.Equal(status, purchase.Status);
        }
    }
}
