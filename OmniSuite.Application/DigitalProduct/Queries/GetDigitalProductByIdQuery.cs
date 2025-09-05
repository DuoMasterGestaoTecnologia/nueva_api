using MediatR;
using OmniSuite.Application.DigitalProduct.Responses;
using OmniSuite.Application.Generic.Responses;

namespace OmniSuite.Application.DigitalProduct.Queries
{
    public class GetDigitalProductByIdQuery : IRequest<Response<DigitalProductResponse>>
    {
        public Guid Id { get; set; }
    }
}
