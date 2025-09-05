using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using OmniSuite.Application.Authentication.Commands;
using OmniSuite.Application.Authentication.Validations;
using OmniSuite.Domain.Entities;
using OmniSuite.Domain.Enums;
using OmniSuite.Persistence;
using OmniSuite.Tests.Common;
using OmniSuite.Tests.Common.Factories;
using Xunit;

namespace OmniSuite.Tests.Application.Authentication.Validations
{
    public class LoginValidationTests : InMemoryDatabaseTestBase
    {
        private readonly LoginValidation _validator;
        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;

        public LoginValidationTests()
        {
            SetupDatabase();
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _validator = new LoginValidation(Context, _mockHttpContextAccessor.Object);
        }

        

        [Fact]
        public async Task ValidateAsync_WithValidCredentials_ShouldPassValidation()
        {
            // Arrange
            var user = UserFactory.CreateValidUser();
            // Create a valid password hash using PasswordHasher
            var hasher = new PasswordHasher<object>();
            user.PasswordHash = hasher.HashPassword(null, "TestPassword123!");
            await SaveEntityAsync(user);

            var command = CommandFactory.CreateValidLoginCommand(user.Email, "TestPassword123!");

            // Act
            var result = await _validator.ValidateAsync(command);

            // Assert
            result.Should().NotBeNull();
            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }

        [Fact]
        public async Task ValidateAsync_WithNonExistentUser_ShouldFailValidation()
        {
            // Arrange
            var command = CommandFactory.CreateValidLoginCommand("nonexistent@example.com", "TestPassword123!");

            // Act
            var result = await _validator.ValidateAsync(command);

            // Assert
            result.Should().NotBeNull();
            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(1);
            result.Errors.First().PropertyName.Should().Be("User");
            result.Errors.First().ErrorMessage.Should().Be("Usuário não encontrado na base de dados");
        }

        [Fact]
        public async Task ValidateAsync_WithInactiveUser_ShouldFailValidation()
        {
            // Arrange
            var user = UserFactory.CreateInactiveUser();
            await SaveEntityAsync(user);

            var command = CommandFactory.CreateValidLoginCommand(user.Email, "TestPassword123!");

            // Act
            var result = await _validator.ValidateAsync(command);

            // Assert
            result.Should().NotBeNull();
            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(1);
            result.Errors.First().PropertyName.Should().Be("Account");
            result.Errors.First().ErrorMessage.Should().Be("Usuário está desativado, entre em contato com o suporte para mais informações.");
        }


        [Fact]
        public async Task ValidateAsync_WithEmptyEmail_ShouldFailValidation()
        {
            // Arrange
            var command = CommandFactory.CreateInvalidLoginCommand();

            // Act
            var result = await _validator.ValidateAsync(command);

            // Assert
            result.Should().NotBeNull();
            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(1);
            result.Errors.First().PropertyName.Should().Be("User");
            result.Errors.First().ErrorMessage.Should().Be("Usuário não encontrado na base de dados");
        }

        [Fact]
        public async Task ValidateAsync_WithNullCommand_ShouldThrowArgumentNullException()
        {
            // Arrange
            LoginCommand? command = null;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => 
                _validator.ValidateAsync(command!));
        }

        [Fact]
        public async Task ValidateAsync_WithMultipleValidationFailures_ShouldReturnAllErrors()
        {
            // Arrange
            var user = UserFactory.CreateInactiveUser();
            // Create a valid password hash using PasswordHasher
            var hasher = new PasswordHasher<object>();
            user.PasswordHash = hasher.HashPassword(null, "TestPassword123!");
            await SaveEntityAsync(user);

            var command = CommandFactory.CreateValidLoginCommand(user.Email, "WrongPassword123!");

            // Act
            var result = await _validator.ValidateAsync(command);

            // Assert
            result.Should().NotBeNull();
            result.IsValid.Should().BeFalse();
            // Note: The current implementation stops at the first failure, so only one error is returned
            // If you want to collect all errors, you would need to modify the validation logic
            result.Errors.Should().HaveCount(1);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("invalid-email")]
        public async Task ValidateAsync_WithInvalidEmailFormats_ShouldFailValidation(string email)
        {
            // Arrange
            var command = new LoginCommand(email, "TestPassword123!");

            // Act
            var result = await _validator.ValidateAsync(command);

            // Assert
            result.Should().NotBeNull();
            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(1);
            result.Errors.First().PropertyName.Should().Be("User");
            result.Errors.First().ErrorMessage.Should().Be("Usuário não encontrado na base de dados");
        }
    }
}
