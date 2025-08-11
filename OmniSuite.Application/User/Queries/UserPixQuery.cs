namespace OmniSuite.Application.User.Queries
{
    public record UserPixQuery() : IRequest<Response<List<UserPixResponse>>>;
}
