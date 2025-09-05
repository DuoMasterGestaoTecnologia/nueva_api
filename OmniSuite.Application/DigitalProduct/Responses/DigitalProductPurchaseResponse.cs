using OmniSuite.Domain.Enums;

namespace OmniSuite.Application.DigitalProduct.Responses
{
    public class DigitalProductPurchaseResponse
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid DigitalProductId { get; set; }
        public string DigitalProductName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime PurchaseDate { get; set; }
        public DigitalProductPurchaseStatusEnum Status { get; set; }
        public string? DownloadToken { get; set; }
        public DateTime? DownloadTokenExpiresAt { get; set; }
        public int DownloadCount { get; set; }
        public string? DownloadUrl { get; set; }
        public string? AccessInstructions { get; set; }
    }
}
