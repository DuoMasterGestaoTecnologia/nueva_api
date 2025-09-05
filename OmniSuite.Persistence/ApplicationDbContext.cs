using Microsoft.EntityFrameworkCore;
using OmniSuite.Domain.Entities;

namespace OmniSuite.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserToken> UserToken { get; set; }
        public DbSet<Deposit> Deposit { get; set; }
        public DbSet<UserBalance> UserBalance { get; set; }
        public DbSet<Withdraw> Withdraw { get; set; }
        public DbSet<Affiliates> Affiliates { get; set; }
        public DbSet<AffiliatesCommission> AffiliatesCommission { get; set; }
        public DbSet<ActiveTransactions> ActiveTransactions { get; set; }
        public DbSet<DigitalProduct> DigitalProducts { get; set; }
        public DbSet<DigitalProductPurchase> DigitalProductPurchases { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Tables
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<UserToken>().ToTable("UserTokens");
            modelBuilder.Entity<Deposit>().ToTable("Deposits");
            modelBuilder.Entity<UserBalance>().ToTable("UserBalances");
            modelBuilder.Entity<Withdraw>().ToTable("Withdraw");
            modelBuilder.Entity<Affiliates>().ToTable("Affiliates");
            modelBuilder.Entity<AffiliatesCommission>().ToTable("AffiliatesCommission");
            modelBuilder.Entity<ActiveTransactions>().ToTable("ActiveTransactions");
            modelBuilder.Entity<DigitalProduct>().ToTable("DigitalProducts");
            modelBuilder.Entity<DigitalProductPurchase>().ToTable("DigitalProductPurchases");

            modelBuilder.Entity<Deposit>()
                .Property(d => d.TransactionStatus)
                .HasConversion<int>();
            
            modelBuilder.Entity<Deposit>()
                .Property(d => d.PaymentMethod)
                .HasConversion<int>();

            modelBuilder.Entity<Withdraw>()
                .Property(d => d.TransactionStatus)
                .HasConversion<int>();

            // DigitalProduct configurations
            modelBuilder.Entity<DigitalProduct>(entity =>
            {
                entity.HasKey(dp => dp.Id);
                
                entity.Property(dp => dp.Name)
                      .IsRequired()
                      .HasMaxLength(200);
                
                entity.Property(dp => dp.Description)
                      .IsRequired()
                      .HasMaxLength(2000);
                
                entity.Property(dp => dp.ShortDescription)
                      .HasMaxLength(500);
                
                entity.Property(dp => dp.Price)
                      .HasColumnType("decimal(18,2)")
                      .IsRequired();
                
                entity.Property(dp => dp.Type)
                      .HasConversion<int>();
                
                entity.Property(dp => dp.Status)
                      .HasConversion<int>();
                
                entity.Property(dp => dp.Category)
                      .HasMaxLength(100);
                
                entity.Property(dp => dp.Tags)
                      .HasMaxLength(500);
                
                entity.Property(dp => dp.CreatedAt)
                      .IsRequired();
                
                entity.HasOne(dp => dp.CreatedByUser)
                      .WithMany()
                      .HasForeignKey(dp => dp.CreatedBy)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // DigitalProductPurchase configurations
            modelBuilder.Entity<DigitalProductPurchase>(entity =>
            {
                entity.HasKey(dpp => dpp.Id);
                
                entity.Property(dpp => dpp.Amount)
                      .HasColumnType("decimal(18,2)")
                      .IsRequired();
                
                entity.Property(dpp => dpp.Status)
                      .HasConversion<int>();
                
                entity.Property(dpp => dpp.PurchaseDate)
                      .IsRequired();
                
                entity.Property(dpp => dpp.DownloadToken)
                      .HasMaxLength(255);
                
                entity.HasOne(dpp => dpp.User)
                      .WithMany()
                      .HasForeignKey(dpp => dpp.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
                
                entity.HasOne(dpp => dpp.DigitalProduct)
                      .WithMany(dp => dp.Purchases)
                      .HasForeignKey(dpp => dpp.DigitalProductId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<UserToken>(entity =>
            {
                entity.HasKey(t => t.Id);

                entity.Property(t => t.Token)
                      .IsRequired()
                      .HasMaxLength(255);

                entity.Property(t => t.Type)
                      .IsRequired()
                      .HasMaxLength(50);

                entity.Property(t => t.CreatedAt)
                      .IsRequired();

                entity.Property(t => t.ExpiresAt)
                      .IsRequired();

                entity.Property(t => t.IsUsed)
                      .IsRequired();

                entity.HasIndex(t => t.Token)
                      .IsUnique();

                entity.HasOne(t => t.User)
                      .WithMany(u => u.Tokens)
                      .HasForeignKey(t => t.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}