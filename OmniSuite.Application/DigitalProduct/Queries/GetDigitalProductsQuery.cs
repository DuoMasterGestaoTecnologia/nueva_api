using MediatR;
using OmniSuite.Application.DigitalProduct.Responses;
using OmniSuite.Application.Generic.Responses;
using OmniSuite.Domain.Enums;

namespace OmniSuite.Application.DigitalProduct.Queries
{
    public class GetDigitalProductsQuery : IRequest<Response<PaginatedResult<DigitalProductResponse>>>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Search { get; set; }
        public DigitalProductTypeEnum? Type { get; set; }
        public DigitalProductStatusEnum? Status { get; set; }
        public Guid? CategoryId { get; set; }
        public bool? IsFeatured { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
    }
}
