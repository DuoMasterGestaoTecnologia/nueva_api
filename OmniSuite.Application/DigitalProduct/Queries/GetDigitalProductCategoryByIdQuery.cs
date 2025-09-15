using MediatR;
using OmniSuite.Application.DigitalProduct.Responses;
using OmniSuite.Application.Generic.Responses;

namespace OmniSuite.Application.DigitalProduct.Queries
{
    public class GetDigitalProductCategoryByIdQuery : IRequest<Response<DigitalProductCategoryResponse>>
    {
        public Guid Id { get; set; }
    }
}
