using FxCreditSystem.Repository.Entities;
using Microsoft.EntityFrameworkCore;

namespace FxCreditSystem.Repository
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) {}
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> AccountHistory { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Account
            modelBuilder
                .Entity<Account>()
                .HasKey(e => e.Id);
            modelBuilder
                .Entity<Account>()
                .HasIndex(e => e.ExternalId)
                .IsUnique();
            modelBuilder
                .Entity<Account>()
                .Property(e => e.Credits)
                .HasConversion<double>();
            modelBuilder
                .Entity<Account>()
                .Property(e => e.Description)
                .HasMaxLength(256)
                .IsUnicode();

            // Transaction
            modelBuilder
                .Entity<Transaction>()
                .HasKey(e => e.Id);
            modelBuilder
                .Entity<Transaction>()
                .HasIndex(e => new { e.AccountId, e.ExternalId })
                .IsUnique();
            modelBuilder
                .Entity<Transaction>()
                .HasIndex(e => new { e.AccountId, e.Id })
                .IsUnique();
            modelBuilder
                .Entity<Transaction>()
                .Property(e => e.CreditsChange)
                .HasConversion<double>();
            modelBuilder
                .Entity<Transaction>()
                .Property(e => e.CreditsNew)
                .HasConversion<double>();
            modelBuilder
                .Entity<Transaction>()
                .Property(e => e.Description)
                .HasMaxLength(256)
                .IsUnicode();

            // Relation Account-Transaction (1:n)
            modelBuilder
                .Entity<Transaction>()
                .HasOne(t => t.Account)
                .WithMany(a => a.Transactions)
                .HasForeignKey(t => t.AccountId);

            // Relation Transaction-Transaction (1:n, but practically 1:1)
            modelBuilder
                .Entity<Transaction>()
                .HasOne(t => t.PrimaryTransaction)
                .WithMany()
                .HasForeignKey(t => t.PrimaryTransactionId);
        }
    }
}