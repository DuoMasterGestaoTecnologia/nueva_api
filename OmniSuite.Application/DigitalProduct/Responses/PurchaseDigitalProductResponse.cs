using OmniSuite.Domain.Enums;

namespace OmniSuite.Application.DigitalProduct.Responses
{
    public class PurchaseDigitalProductResponse
    {
        public Guid PurchaseId { get; set; }
        public Guid DigitalProductId { get; set; }
        public string DigitalProductName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DigitalProductPurchaseStatusEnum Status { get; set; }
        public string? DownloadToken { get; set; }
        public DateTime? DownloadTokenExpiresAt { get; set; }
        public string? DownloadUrl { get; set; }
        public string? AccessInstructions { get; set; }
        public DateTime PurchaseDate { get; set; }
    }
}
