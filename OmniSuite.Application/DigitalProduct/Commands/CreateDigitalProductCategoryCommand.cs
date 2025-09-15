using MediatR;
using OmniSuite.Application.DigitalProduct.Responses;
using OmniSuite.Application.Generic.Responses;

namespace OmniSuite.Application.DigitalProduct.Commands
{
    public class CreateDigitalProductCategoryCommand : IRequest<Response<DigitalProductCategoryResponse>>
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? IconUrl { get; set; }
        public string? Color { get; set; }
        public int SortOrder { get; set; } = 0;
    }
}
