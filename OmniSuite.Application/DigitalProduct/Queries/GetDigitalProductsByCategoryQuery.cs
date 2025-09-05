using MediatR;
using OmniSuite.Application.DigitalProduct.Responses;
using OmniSuite.Application.Generic.Responses;

namespace OmniSuite.Application.DigitalProduct.Queries
{
    public class GetDigitalProductsByCategoryQuery : IRequest<Response<PaginatedResult<DigitalProductResponse>>>
    {
        public string Category { get; set; } = string.Empty;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
