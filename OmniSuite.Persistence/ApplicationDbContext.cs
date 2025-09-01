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

            modelBuilder.Entity<Deposit>()
                .Property(d => d.TransactionStatus)
                .HasConversion<int>();
            
            modelBuilder.Entity<Deposit>()
                .Property(d => d.PaymentMethod)
                .HasConversion<int>();

            modelBuilder.Entity<Withdraw>()
                .Property(d => d.TransactionStatus)
                .HasConversion<int>();

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