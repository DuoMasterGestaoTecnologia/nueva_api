using OmniSuite.Application.Authentication.Commands;
using OmniSuite.Application.User.Commands;

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
            return new UpdateUserCommand(
                name ?? "Updated User Name",
                email ?? "updated@example.com",
                passwordHash ?? "new_hashed_password",
                Domain.Enums.UserStatusEnum.active,
                "new_refresh_token",
                DateTime.UtcNow.AddDays(7),
                "JBSWY3DPEHPK3PXP",
                true,
                "+5511888888888",
                "98765432109"
            );
        }

        public static UpdatePhotoUserCommand CreateValidUpdatePhotoCommand()
        {
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.FileName).Returns("test.jpg");
            mockFile.Setup(f => f.ContentType).Returns("image/jpeg");
            mockFile.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                   .Returns(Task.CompletedTask);

            return new UpdatePhotoUserCommand(mockFile.Object);
        }
    }
}
