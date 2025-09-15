using MediatR;
using OmniSuite.Application.Generic.Responses;

namespace OmniSuite.Application.DigitalProduct.Commands
{
    public class DeleteDigitalProductCategoryCommand : IRequest<Response<bool>>
    {
        public Guid Id { get; set; }
    }
}
