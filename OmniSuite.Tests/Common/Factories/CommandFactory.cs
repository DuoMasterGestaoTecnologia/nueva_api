using OmniSuite.Application.Authentication.Commands;
using OmniSuite.Application.User.Commands;
using Microsoft.AspNetCore.Http;
using Moq;

namespace OmniSuite.Tests.Common.Factories
{
    public static class CommandFactory
    {
        public static LoginCommand CreateValidLoginCommand(
            string? email = null,
            string? password = null)
        {
            return new LoginCommand(
                email ?? "test@example.com",
                password ?? "TestPassword123!"
            );
        }

        public static LoginCommand CreateInvalidLoginCommand()
        {
            return new LoginCommand("", "");
        }

        public static RefreshTokenCommand CreateValidRefreshTokenCommand(
            string? refreshToken = null)
        {
            return new RefreshTokenCommand(
                refreshToken ?? "valid_refresh_token_123"
            );
        }

        public static LogoutCommand CreateLogoutCommand()
        {
            return new LogoutCommand();
        }

        public static CreateMFAUserCommand CreateValidMFACommand(
            string? secret = null,
            string? code = null)
        {
            return new CreateMFAUserCommand(
                secret ?? "JBSWY3DPEHPK3PXP",
                code ?? "123456"
            );
        }

        public static UpdateUserCommand CreateValidUpdateUserCommand(
            string? name = null,
            string? email = null,
            string? passwordHash = null)
        {
            return new UpdateUserCommand
            {
                Id = Guid.NewGuid(),
                Name = name ?? "Updated User Name",
                Email = email ?? "updated@example.com",
                PasswordHash = passwordHash ?? "new_hashed_password",
                Status = OmniSuite.Domain.Enums.UserStatusEnum.approved,
                RefreshToken = "new_refresh_token",
                RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(7),
                MfaSecretKey = "JBSWY3DPEHPK3PXP",
                IsMfaEnabled = true,
                Phone = "+5511888888888",
                Document = "98765432109"
            };
        }

        public static UpdatePhotoUserCommand CreateValidUpdatePhotoCommand()
        {
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.FileName).Returns("test.jpg");
            mockFile.Setup(f => f.ContentType).Returns("image/jpeg");
            mockFile.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                   .Returns(Task.CompletedTask);

            return new UpdatePhotoUserCommand
            {
                DocumentImageBase64 = mockFile.Object
            };
        }
    }
}
