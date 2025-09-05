using MediatR;
using OmniSuite.Application.DigitalProduct.Responses;
using OmniSuite.Application.Generic.Responses;

namespace OmniSuite.Application.DigitalProduct.Commands
{
    public class PurchaseDigitalProductCommand : IRequest<Response<PurchaseDigitalProductResponse>>
    {
        public Guid DigitalProductId { get; set; }
    }
}
