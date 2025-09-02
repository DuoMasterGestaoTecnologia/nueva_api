using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OmniSuite.Application.User;
using OmniSuite.Application.User.Commands;
using OmniSuite.Application.User.Queries;
using OmniSuite.Application.User.Query;
using OmniSuite.Application.User.Responses;
using OmniSuite.Domain.Entities;
using OmniSuite.Domain.Interfaces;
using OmniSuite.Domain.Utils;
using OmniSuite.Persistence;
using OmniSuite.Tests.Common;
using OmniSuite.Tests.Common.Factories;
using Xunit;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace OmniSuite.Tests.Application.User
{
    public class UserQueryHandlerTests : InMemoryDatabaseTestBase
    {
        private readonly UserQueryHandler _handler;
        private readonly Mock<IEmailService> _mockEmailService;
        private readonly Mock<IMfaService> _mockMfaService;

        public UserQueryHandlerTests()
        {
            SetupDatabase();
            _mockEmailService = new Mock<IEmailService>();
            _mockMfaService = new Mock<IMfaService>();
            _handler = new UserQueryHandler(Context, _mockEmailService.Object, _mockMfaService.Object);
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
        public async Task Handle_UserByEmailQuery_WithValidEmail_ShouldReturnUserResponse()
        {
            // Arrange
            var user = UserFactory.CreateValidUser();
            await SaveEntityAsync(user);

            var query = new UserByEmailQuery(user.Email);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(user.Id);
            result.Nome.Should().Be(user.Name);
            result.email.Should().Be(user.Email);
        }

        [Fact]
        public async Task Handle_UserByEmailQuery_WithInvalidEmail_ShouldReturnNull()
        {
            // Arrange
            var query = new UserByEmailQuery("nonexistent@example.com");

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task Handle_UpdateUserCommand_ShouldUpdateUserSuccessfully()
        {
            // Arrange
            var user = UserFactory.CreateValidUser();
            await SaveEntityAsync(user);

            var command = CommandFactory.CreateValidUpdateUserCommand();

            // Setup UserClaimsHelper to return the user ID
            SetupUserClaimsHelper(user.Id);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Data.Should().BeTrue();

            // Verify the user was updated in the database
            var updatedUser = await Context.Users.FindAsync(user.Id);
            updatedUser.Should().NotBeNull();
            updatedUser!.Name.Should().Be(command.Name);
            updatedUser.Email.Should().Be(command.Email);

        }

        [Fact]
        public async Task Handle_UpdatePhotoUserCommand_ShouldUpdateProfilePhotoSuccessfully()
        {
            // Arrange
            var user = UserFactory.CreateValidUser();
            await SaveEntityAsync(user);

            var command = CommandFactory.CreateValidUpdatePhotoCommand();

            // Setup UserClaimsHelper to return the user ID
            SetupUserClaimsHelper(user.Id);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeTrue();

            // Verify the profile photo was updated in the database
            var updatedUser = await Context.Users.FindAsync(user.Id);
            updatedUser.Should().NotBeNull();
            updatedUser!.ProfilePhoto.Should().NotBeNull();
            updatedUser.ProfilePhoto.Should().Contain("test.jpg");
            updatedUser.ProfilePhoto.Should().Contain("image/jpeg");
        }

        [Fact]
        public async Task Handle_CreateMFAUserCommand_WithValidCode_ShouldEnableMFA()
        {
            // Arrange
            var user = UserFactory.CreateValidUser();
            await SaveEntityAsync(user);

            var command = CommandFactory.CreateValidMFACommand();

            // Setup UserClaimsHelper to return the user ID
            SetupUserClaimsHelper(user.Id);

            // Setup MFA service to return true for validation
            _mockMfaService.Setup(x => x.ValidateCode(It.IsAny<string>(), It.IsAny<string>()))
                          .Returns(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data!.UserId.Should().Be(user.Id);
            result.Data.sucess.Should().BeTrue();
        }

        [Fact]
        public async Task Handle_CreateMFAUserCommand_WithInvalidCode_ShouldReturnFailure()
        {
            // Arrange
            var user = UserFactory.CreateValidUser();
            await SaveEntityAsync(user);

            var command = CommandFactory.CreateValidMFACommand();

            // Setup UserClaimsHelper to return the user ID
            SetupUserClaimsHelper(user.Id);

            // Mock MfaService to return invalid code
            // Note: In a real test, you might need to mock the static MfaService or use a different approach

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.Data.Should().BeNull();
            result.Message.Should().Be("Código inválido");
        }

        [Fact]
        public async Task Handle_SetupMFAQuery_ShouldReturnMFAConfiguration()
        {
            // Arrange
            var user = UserFactory.CreateValidUser();
            await SaveEntityAsync(user);

            var query = new SetupMFAQuery();

            // Setup UserClaimsHelper to return the user ID
            SetupUserClaimsHelper(user.Id);

            // Setup MFA service to return mock values
            _mockMfaService.Setup(x => x.GenerateMfaSecret(It.IsAny<string>()))
                          .Returns(("test-secret", "test-uri"));
            _mockMfaService.Setup(x => x.GenerateQrCodeSvg(It.IsAny<string>()))
                          .Returns("test-qr-svg");

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data!.Secret.Should().NotBeNullOrEmpty();
            result.Data.QrCodeSvg.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task Handle_UserLoggedQuery_WithValidUser_ShouldReturnUserLoggedResponse()
        {
            // Arrange
            var user = UserFactory.CreateValidUser();
            await SaveEntityAsync(user);

            var query = new UserLoggedQuery();

            // Setup UserClaimsHelper to return the user ID
            SetupUserClaimsHelper(user.Id);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data!.Id.Should().Be(user.Id);
            result.Data.Name.Should().Be(user.Name);
            result.Data.Email.Should().Be(user.Email);
            result.Data.Amount.Should().Be(user.UserBalance.TotalAmount);
        }

        [Fact]
        public async Task Handle_UserLoggedQuery_WithNonExistentUser_ShouldReturnFailure()
        {
            // Arrange
            var query = new UserLoggedQuery();

            // Mock UserClaimsHelper to return a non-existent user ID
            // In a real test, you might need to mock the HttpContext or use a different approach

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.Data.Should().BeNull();
            result.Message.Should().Be("Nenhum usuario encontrado");
        }

        [Fact]
        public async Task Handle_GetUserQuery_WithValidUser_ShouldReturnGetUserResponse()
        {
            // Arrange
            var user = UserFactory.CreateValidUser();
            await SaveEntityAsync(user);

            var query = new GetUserQuery();

            // Setup UserClaimsHelper to return the user ID
            SetupUserClaimsHelper(user.Id);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data!.Id.Should().Be(user.Id);
            result.Data.Name.Should().Be(user.Name);
            result.Data.Email.Should().Be(user.Email);
            result.Data.Phone.Should().Be(user.Phone);
            result.Data.DocumentNumber.Should().Be(user.Document);
            result.Data.CreatedAt.Should().Be(user.CreatedAt);
            result.Data.Status.Should().Be(user.Status);
            result.Data.ProfilePhoto.Should().Be(user.ProfilePhoto);
            result.Data.Amount.Should().Be(user.UserBalance.TotalAmount);
        }

        [Fact]
        public async Task Handle_GetUserQuery_WithNonExistentUser_ShouldReturnFailure()
        {
            // Arrange
            var query = new GetUserQuery();

            // Mock UserClaimsHelper to return a non-existent user ID
            // In a real test, you might need to mock the HttpContext or use a different approach

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.Data.Should().BeNull();
            result.Message.Should().Be("Nenhum usuario encontrado");
        }
    }
}
