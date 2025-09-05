namespace OmniSuite.Domain.Entities
{
    public class DigitalProductPurchase
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid DigitalProductId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PurchaseDate { get; set; }
        public DigitalProductPurchaseStatusEnum Status { get; set; }
        public string? DownloadToken { get; set; }
        public DateTime? DownloadTokenExpiresAt { get; set; }
        public int DownloadCount { get; set; } = 0;

        // Relacionamentos
        public User User { get; set; } = default!;
        public DigitalProduct DigitalProduct { get; set; } = default!;
    }
}
