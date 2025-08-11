namespace OmniSuite.Application.User.Queries
{
    public record GetUserQuery() : IRequest<Response<GetUserResponse>>;
}
