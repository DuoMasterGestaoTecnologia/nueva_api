namespace OmniSuite.Application.Generic.Queries
{
    public static class IQueryableExtensions
    {
        public static async Task<PaginatedResult<T>> PaginateAsync<T>(
            this IQueryable<T> query,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var totalItems = await query.CountAsync(cancellationToken);
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PaginatedResult<T>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems
            };
        }
    }

}
