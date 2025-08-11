namespace OmniSuite.Application.Common
{
    public class ValidationBase<TRequest> : AbstractValidator<TRequest>
    {
        internal readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContext;

        public ValidationBase(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContext = httpContextAccessor;
        }
    }
}
