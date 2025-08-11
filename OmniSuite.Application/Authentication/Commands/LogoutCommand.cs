namespace OmniSuite.Application.Authentication.Commands
{
    public record LogoutCommand() : IRequest<Response<bool>>;
}
