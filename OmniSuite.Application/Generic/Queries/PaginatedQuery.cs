namespace OmniSuite.Application.Generic.Queries
{
    public abstract class PaginatedQuery<TResponse> : IRequest<Response<PaginatedResult<TResponse>>>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
