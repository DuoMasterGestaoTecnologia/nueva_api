namespace OmniSuite.Domain.Entities
{
    public class DigitalProduct
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? ShortDescription { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public string? ThumbnailUrl { get; set; }
        public DigitalProductTypeEnum Type { get; set; }
        public DigitalProductStatusEnum Status { get; set; } = DigitalProductStatusEnum.Active;
        public string? DownloadUrl { get; set; }
        public string? AccessInstructions { get; set; }
        public int? DownloadLimit { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public bool IsFeatured { get; set; } = false;
        public bool IsDigitalDelivery { get; set; } = true;
        public string? Tags { get; set; }
        public Guid? CategoryId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? CreatedBy { get; set; }

        // Relacionamentos
        public User? CreatedByUser { get; set; }
        public DigitalProductCategory? Category { get; set; }
        public ICollection<DigitalProductPurchase> Purchases { get; set; } = new List<DigitalProductPurchase>();
    }
}
