namespace OmniSuite.Application.User.Commands
{
    public record CreateMFAUserCommand(string Secret, string Code) : IRequest<Response<CreateMFAUserResponse>>;
}
