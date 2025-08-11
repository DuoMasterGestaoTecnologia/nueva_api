namespace OmniSuite.Application.User.Query
{
    public record UserByEmailQuery(string email) : IRequest<UserByEmailResponse>;
}
