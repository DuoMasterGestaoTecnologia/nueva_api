namespace OmniSuite.Domain.Entities
{
    public class DigitalProductCategory
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? IconUrl { get; set; }
        public string? Color { get; set; }
        public bool IsActive { get; set; } = true;
        public int SortOrder { get; set; } = 0;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? CreatedBy { get; set; }

        // Relacionamentos
        public User? CreatedByUser { get; set; }
        public ICollection<DigitalProduct> DigitalProducts { get; set; } = new List<DigitalProduct>();
    }
}
