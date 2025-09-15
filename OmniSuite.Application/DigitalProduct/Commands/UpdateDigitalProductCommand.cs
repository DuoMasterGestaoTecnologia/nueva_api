using MediatR;
using OmniSuite.Application.DigitalProduct.Responses;
using OmniSuite.Application.Generic.Responses;
using OmniSuite.Domain.Enums;

namespace OmniSuite.Application.DigitalProduct.Commands
{
    public class UpdateDigitalProductCommand : IRequest<Response<DigitalProductResponse>>
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? ShortDescription { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public string? ThumbnailUrl { get; set; }
        public DigitalProductTypeEnum Type { get; set; }
        public DigitalProductStatusEnum Status { get; set; }
        public string? DownloadUrl { get; set; }
        public string? AccessInstructions { get; set; }
        public int? DownloadLimit { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public bool IsFeatured { get; set; } = false;
        public bool IsDigitalDelivery { get; set; } = true;
        public Guid? CategoryId { get; set; }
        public string? Tags { get; set; }
    }
}
