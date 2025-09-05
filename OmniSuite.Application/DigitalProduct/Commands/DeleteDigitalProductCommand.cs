using MediatR;
using OmniSuite.Application.Generic.Responses;

namespace OmniSuite.Application.DigitalProduct.Commands
{
    public class DeleteDigitalProductCommand : IRequest<Response<bool>>
    {
        public Guid Id { get; set; }
    }
}
