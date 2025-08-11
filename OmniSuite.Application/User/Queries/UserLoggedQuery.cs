namespace OmniSuite.Application.User.Queries
{
    public record UserLoggedQuery() : IRequest<Response<UserLoggedResponse>>;
}
