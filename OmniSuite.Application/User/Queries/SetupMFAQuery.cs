namespace OmniSuite.Application.User.Queries
{
    public record SetupMFAQuery() : IRequest<Response<SetupMFAResponse>>;
}
