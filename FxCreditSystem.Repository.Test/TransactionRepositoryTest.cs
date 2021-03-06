using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FxCreditSystem.Repository.Test
{
    public class TransactionRepositoryTest
    {
        private readonly DbContextOptions<DataContext> dbContextOptions;
        private int accountInternalId;
        private Guid accountId;
        private int otherAccountInternalId;
        private Guid otherAccountId;

        public TransactionRepositoryTest()
        {
            dbContextOptions = new DbContextOptionsBuilder<DataContext>()
                .UseSqlite("Filename=FxCreditSystem.Repository.Test.db")
                .Options;

            Seed();
        }

        private void Seed()
        {
            using (var dbContext = new DataContext(dbContextOptions))
            {
                dbContext.Database.EnsureDeleted();
                dbContext.Database.EnsureCreated();

                accountId = Guid.NewGuid();
                otherAccountId = Guid.NewGuid();
                var account = new Entities.Account {
                    ExternalId = accountId,
                    Description = "Account 1",
                    MinimumCredits = -10.0m,
                    Credits = 100.0m,
                };
                var otherAccount = new Entities.Account {
                    ExternalId = otherAccountId,
                    Description = "Account 2",
                    MinimumCredits = -20.0m,
                    Credits = 120.0m,
                };
                dbContext.Accounts.AddRange(
                    account,
                    otherAccount
                );

                dbContext.SaveChanges();

                accountInternalId = account.Id;
                otherAccountInternalId = otherAccount.Id;
            }
        }

        [Fact]
        public async Task AddTransfer_FromUnknownAccount_ShouldFail()
        {
            using (var dbContext = new DataContext(dbContextOptions))
            {
                var transactionReposity = new FxCreditSystem.Repository.TransactionRepository(dbContext);

                Guid unknownAccountId = Guid.NewGuid();
                Guid transactionId = Guid.NewGuid();
                DateTime now = DateTime.UtcNow;

                await Assert.ThrowsAsync<AccountNotFoundException>(async () => 
                    await transactionReposity.Add(unknownAccountId, transactionId, now, "Test", -40.0m, otherAccountId));
            }
        }

        [Fact]
        public async Task AddTransfer_ToUnknownAccount_ShouldFail()
        {
            using (var dbContext = new DataContext(dbContextOptions))
            {
                var transactionReposity = new FxCreditSystem.Repository.TransactionRepository(dbContext);

                Guid unknownAccountId = Guid.NewGuid();
                Guid transactionId = Guid.NewGuid();
                DateTime now = DateTime.UtcNow;

                await Assert.ThrowsAsync<AccountNotFoundException>(async () => 
                    await transactionReposity.Add(accountId, transactionId, now, "Test", -40.0m, unknownAccountId));
            }
        }

        [Fact]
        public async Task AddTransfer_BelowMinimumCredits_ShouldFail()
        {
            using (var dbContext = new DataContext(dbContextOptions))
            {
                var transactionReposity = new FxCreditSystem.Repository.TransactionRepository(dbContext);

                Guid transactionId = Guid.NewGuid();
                DateTime now = DateTime.UtcNow;

                await Assert.ThrowsAsync<AccountCreditsInsufficientException>(async () => 
                    await transactionReposity.Add(accountId, transactionId, now, "Test", -111.0m, otherAccountId));
            }
        }

        [Fact]
        public async Task AddTransfer_TakingCreditsFromOtherAccount_ShouldFail()
        {
            using (var dbContext = new DataContext(dbContextOptions))
            {
                var transactionReposity = new FxCreditSystem.Repository.TransactionRepository(dbContext);

                Guid transactionId = Guid.NewGuid();
                DateTime now = DateTime.UtcNow;

                await Assert.ThrowsAsync<DebetFromOtherAccountNotAllowedException>(async () => 
                    await transactionReposity.Add(accountId, transactionId, now, "Test", +1.0m, otherAccountId));
            }
        }

        [Fact]
        public async Task AddTransfer_ShouldSucceed()
        {
            using (var dbContext = new DataContext(dbContextOptions))
            {
                var transactionReposity = new FxCreditSystem.Repository.TransactionRepository(dbContext);

                Guid transactionId = Guid.NewGuid();
                DateTime now = DateTime.UtcNow;

                await transactionReposity.Add(accountId, transactionId, now, "Test", -12.23m, otherAccountId);

                var account = await dbContext.Accounts.FindAsync(accountInternalId);
                Assert.Equal(now, account.LastChangeUtc);
                Assert.Equal(87.77m, account.Credits);
                var transaction = await dbContext.AccountHistory.AsQueryable().Where(t => t.Account.Id == account.Id).OrderByDescending(t => t.Id).FirstAsync();
                Assert.Equal(-12.23m, transaction.CreditsChange);
                Assert.Equal(87.77m, transaction.CreditsNew);
            }
        }
    }
}
