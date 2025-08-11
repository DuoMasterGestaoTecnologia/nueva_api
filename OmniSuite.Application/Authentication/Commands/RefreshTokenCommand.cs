namespace OmniSuite.Application.Authentication.Commands
{
    public record RefreshTokenCommand(string refreshToken) : IRequest<Response<AuthenticationResponse>>;
}
