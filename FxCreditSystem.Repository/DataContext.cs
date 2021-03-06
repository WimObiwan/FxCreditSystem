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
            modelBuilder
                .Entity<Account>()
                .HasKey(e => e.Id);
            modelBuilder
                .Entity<Account>()
                .Property(e => e.Credits)
                .HasConversion<double>();

            modelBuilder
                .Entity<Transaction>()
                .HasKey(e => e.Id);
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

            modelBuilder
                .Entity<Transaction>()
                .HasOne(e => e.Account)
                .WithMany(e => e.Transactions);
        }
    }
}