using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FxCreditSystem.Repository.Test
{
    public class TransactionRepositoryTest : IDisposable
    {
        private readonly DbContextOptions<DataContext> dbContextOptions;

        private string authUserId;
        private string otherAuthUserId;
        private long accountInternalId;
        private Guid accountId;
        private long otherAccountInternalId;
        private Guid otherAccountId;

        private bool disposedValue;
        private readonly DataContext dbContext;
        private readonly FxCreditSystem.Repository.AccountUserRepository accountUserRepository;
        private readonly FxCreditSystem.Repository.TransactionRepository transactionRepository;

        public TransactionRepositoryTest()
        {
            dbContextOptions = new DbContextOptionsBuilder<DataContext>()
                .UseSqlite("Filename=FxCreditSystem.Repository.Test.db")
                .Options;

            dbContext = new DataContext(dbContextOptions);
            accountUserRepository = new FxCreditSystem.Repository.AccountUserRepository(dbContext);
            transactionRepository = new FxCreditSystem.Repository.TransactionRepository(dbContext, accountUserRepository);

            Seed();
        }

        private void Seed()
        {
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();

            authUserId = $"Test|{Guid.NewGuid().ToString()}";
            otherAuthUserId = $"Test|{Guid.NewGuid().ToString()}";
            var user = new Entities.User()
            {
                AuthUserId = authUserId,
                Description = "Test",                        
            };
            var otherUser = new Entities.User()
            {
                AuthUserId = otherAuthUserId,
                Description = "Test Other",                        
            };
            dbContext.Users.AddRange(
                user,
                otherUser
            );

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

            dbContext.AccountUsers.AddRange(
                new Entities.AccountUser
                {
                    Account = account,
                    User = user,
                },
                new Entities.AccountUser
                {
                    Account = otherAccount,
                    User = otherUser,
                }
            );

            dbContext.SaveChanges();

            accountInternalId = account.Id;
            otherAccountInternalId = otherAccount.Id;
        }

        [Fact]
        public async Task AddTransfer_FromUnknownAccount_ShouldFail()
        {
            Guid unknownAccountId = Guid.NewGuid();
            Guid transactionId = Guid.NewGuid();
            DateTime now = DateTime.UtcNow;

            await Assert.ThrowsAsync<AccountNotFoundException>(async () => 
                await transactionRepository.Add(authUserId, unknownAccountId, transactionId, now, "Test", -40.0m, otherAccountId));
        }

        [Fact]
        public async Task AddTransfer_SomebodyElsesAccount_ShouldFail()
        {
            Guid unknownAccountId = Guid.NewGuid();
            Guid transactionId = Guid.NewGuid();
            DateTime now = DateTime.UtcNow;

            await Assert.ThrowsAsync<AccountNotFoundException>(async () => 
                await transactionRepository.Add(otherAuthUserId, accountId, transactionId, now, "Test", -40.0m, otherAccountId));
        }

        [Fact]
        public async Task AddTransfer_ToUnknownAccount_ShouldFail()
        {
            Guid unknownAccountId = Guid.NewGuid();
            Guid transactionId = Guid.NewGuid();
            DateTime now = DateTime.UtcNow;

            await Assert.ThrowsAsync<AccountNotFoundException>(async () => 
                await transactionRepository.Add(authUserId, accountId, transactionId, now, "Test", -40.0m, unknownAccountId));
        }

        [Fact]
        public async Task AddTransfer_BelowMinimumCredits_ShouldFail()
        {
            Guid transactionId = Guid.NewGuid();
            DateTime now = DateTime.UtcNow;

            await Assert.ThrowsAsync<AccountCreditsInsufficientException>(async () => 
                await transactionRepository.Add(authUserId, accountId, transactionId, now, "Test", -111.0m, otherAccountId));
        }

        [Fact]
        public async Task AddTransfer_TakingCreditsFromOtherAccount_ShouldFail()
        {
            Guid transactionId = Guid.NewGuid();
            DateTime now = DateTime.UtcNow;

            await Assert.ThrowsAsync<DebetFromOtherAccountNotAllowedException>(async () => 
                await transactionRepository.Add(authUserId, accountId, transactionId, now, "Test", +1.0m, otherAccountId));
        }

        [Fact]
        public async Task AddTransfer_ShouldSucceed()
        {
            Guid transactionId = Guid.NewGuid();
            DateTime now = DateTime.UtcNow;
            string description = "Test";

            await transactionRepository.Add(authUserId, accountId, transactionId, now, description, -12.23m, otherAccountId);

            var account = await dbContext.Accounts.FindAsync(accountInternalId);
            Assert.Equal(now, account.LastChangeUtc);
            Assert.Equal(87.77m, account.Credits);

            var transaction = await dbContext.AccountHistory.Where(t => t.AccountId == account.Id).OrderByDescending(t => t.Id).FirstAsync();
            Assert.Equal(-12.23m, transaction.CreditsChange);
            Assert.Equal(87.77m, transaction.CreditsNew);
            Assert.Equal(transactionId, transaction.ExternalId);
            Assert.Equal(description, transaction.Description);
            Assert.Equal(now, transaction.DateTimeUtc);

            var otherAccount = await dbContext.Accounts.FindAsync(otherAccountInternalId);
            Assert.Equal(now, otherAccount.LastChangeUtc);
            Assert.Equal(132.23m, otherAccount.Credits);

            var otherTransaction = await dbContext.AccountHistory.Where(t => t.AccountId == otherAccount.Id).OrderByDescending(t => t.Id).FirstAsync();
            Assert.Equal(12.23m, otherTransaction.CreditsChange);
            Assert.Equal(132.23m, otherTransaction.CreditsNew);
            Assert.Equal(transactionId, otherTransaction.ExternalId);
            Assert.Equal(description, otherTransaction.Description);
            Assert.Equal(now, otherTransaction.DateTimeUtc);
            
            Assert.Equal(transaction.Id, otherTransaction.PrimaryTransactionId);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    dbContext.Dispose();
                }

                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~TransactionRepositoryTest()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
