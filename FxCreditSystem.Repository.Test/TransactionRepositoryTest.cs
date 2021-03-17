using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FxCreditSystem.Repository.Test
{
    public class TransactionRepositoryTest : IDisposable
    {
        private readonly DbContextOptions<DataContext> dbContextOptions;

        private string authUserId;
        private string otherAuthUserId;
        private Entities.Account account;
        private Entities.Account otherAccount;

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
            IMapper mapper = new MapperConfiguration(c => c.AddProfile<AutoMapperProfile>()).CreateMapper();
            accountUserRepository = new FxCreditSystem.Repository.AccountUserRepository(dbContext, mapper);
            transactionRepository = new FxCreditSystem.Repository.TransactionRepository(dbContext, accountUserRepository, mapper);

            Seed();
        }

        private void Seed()
        {
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();

            var userFaker = new Fakers.UserFaker(); 
            var user = userFaker.Generate();
            var otherUser = userFaker.Generate();
            authUserId = user.AuthUserId;
            otherAuthUserId = otherUser.AuthUserId;
            dbContext.Users.AddRange(
                user,
                otherUser
            );

            Fakers.AccountFaker accountFaker = new Fakers.AccountFaker();
            account = accountFaker.Generate();
            otherAccount = accountFaker.Generate();

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

            var transactionFaker = new Fakers.TransactionFaker();
            var transaction = transactionFaker
                .RuleFor(t => t.Account, account)
                .RuleFor(t => t.CreditsChange, account.Credits)
                .RuleFor(t => t.CreditsNew, account.Credits)
                .Generate();
            var otherTransaction = transactionFaker
                .RuleFor(t => t.Account, otherAccount)
                .RuleFor(t => t.CreditsChange, otherAccount.Credits)
                .RuleFor(t => t.CreditsNew, otherAccount.Credits)
                .RuleFor(t => t.PrimaryTransaction, transaction)
                .Generate();

            dbContext.AddRange(
                transaction,
                otherTransaction
            );

            dbContext.SaveChanges();

            dbContext.Entry(account).State = EntityState.Detached;
            dbContext.Entry(otherAccount).State = EntityState.Detached;
        }

        [Fact]
        public async Task AddTransfer_WithInvalidArguments_ShouldFail()
        {
            var transactionAddFaker = new Common.Fakers.TransactionAddFaker();

            await Assert.ThrowsAsync<ArgumentException>(async () => 
                await transactionRepository.Add(
                    transactionAddFaker
                        .RuleFor(ta => ta.AuthUserId, (string)null)
                        .Generate()));

            await Assert.ThrowsAsync<ArgumentException>(async () => 
                await transactionRepository.Add(
                    transactionAddFaker
                        .RuleFor(ta => ta.AuthUserId, "")
                        .Generate()));

            await Assert.ThrowsAsync<ArgumentException>(async () => 
                await transactionRepository.Add(
                    transactionAddFaker
                        .RuleFor(ta => ta.CreditsChange, 0m)
                        .Generate()));
        }

        [Fact]
        public async Task AddTransfer_BetweenSameAccounts_ShouldFail()
        {
            var transactionAddFaker = new Common.Fakers.TransactionAddFaker()
                .RuleFor(ta => ta.AuthUserId, authUserId)
                .RuleFor(ta => ta.AccountId, account.ExternalId)
                .RuleFor(ta => ta.OtherAccountId, account.ExternalId);

            var transactionAdd = transactionAddFaker.Generate();

            await Assert.ThrowsAsync<TransactionBetweenSameAccountsException>(async () => 
                await transactionRepository.Add(transactionAdd));
        }

        [Fact]
        public async Task AddTransfer_FromUnknownAccount_ShouldFail()
        {
            var transactionAddFaker = new Common.Fakers.TransactionAddFaker()
                .RuleFor(ta => ta.AuthUserId, authUserId)
                .RuleFor(ta => ta.OtherAccountId, otherAccount.ExternalId);

            var transactionAdd = transactionAddFaker.Generate();

            await Assert.ThrowsAsync<AccountNotFoundException>(async () => 
                await transactionRepository.Add(transactionAdd));
        }

        [Fact]
        public async Task AddTransfer_SomebodyElsesAccount_ShouldFail()
        {
            var transactionAddFaker = new Common.Fakers.TransactionAddFaker()
                .RuleFor(ta => ta.AuthUserId, otherAuthUserId)
                .RuleFor(ta => ta.AccountId, account.ExternalId)
                .RuleFor(ta => ta.OtherAccountId, otherAccount.ExternalId);

            var transactionAdd = transactionAddFaker.Generate();

            await Assert.ThrowsAsync<AccountNotFoundException>(async () => 
                await transactionRepository.Add(transactionAdd));
        }

        [Fact]
        public async Task AddTransfer_ToUnknownAccount_ShouldFail()
        {
            var transactionAddFaker = new Common.Fakers.TransactionAddFaker()
                .RuleFor(ta => ta.AuthUserId, authUserId)
                .RuleFor(ta => ta.AccountId, account.ExternalId);

            var transactionAdd = transactionAddFaker.Generate();

            await Assert.ThrowsAsync<AccountNotFoundException>(async () => 
                await transactionRepository.Add(transactionAdd));
        }

        [Fact]
        public async Task AddTransfer_BelowMinimumCredits_ShouldFail()
        {
            var transactionAddFaker = new Common.Fakers.TransactionAddFaker()
                .RuleFor(ta => ta.AuthUserId, authUserId)
                .RuleFor(ta => ta.AccountId, account.ExternalId)
                .RuleFor(ta => ta.OtherAccountId, otherAccount.ExternalId)
                .RuleFor(ta => ta.CreditsChange, f => f.Random.Decimal(-account.Credits - 10.0m - 0.01m, -account.Credits - 10.0m - 50m));

            var transactionAdd = transactionAddFaker.Generate();

            await Assert.ThrowsAsync<AccountCreditsInsufficientException>(async () => 
                await transactionRepository.Add(transactionAdd));
        }

        [Fact]
        public async Task AddTransfer_TakingCreditsFromOtherAccount_ShouldFail()
        {
            var transactionAddFaker = new Common.Fakers.TransactionAddFaker()
                .RuleFor(ta => ta.AuthUserId, authUserId)
                .RuleFor(ta => ta.AccountId, account.ExternalId)
                .RuleFor(ta => ta.OtherAccountId, otherAccount.ExternalId)
                .RuleFor(ta => ta.CreditsChange, f => f.Random.Decimal(+1m, +50m));

            var transactionAdd = transactionAddFaker.Generate();

            await Assert.ThrowsAsync<DebetFromOtherAccountNotAllowedException>(async () => 
                await transactionRepository.Add(transactionAdd));
        }

        [Fact]
        public async Task AddTransfer_WithSameTransactionId_ShouldFail()
        {
            var transactionAddFaker = new Common.Fakers.TransactionAddFaker()
                .RuleFor(ta => ta.AuthUserId, authUserId)
                .RuleFor(ta => ta.AccountId, account.ExternalId)
                .RuleFor(ta => ta.OtherAccountId, otherAccount.ExternalId);

            var transaction1 = transactionAddFaker.Generate();
            var transaction2 = transactionAddFaker.RuleFor(ta => ta.TransactionId, transaction1.TransactionId).Generate();

            await transactionRepository.Add(transaction1);
            await Assert.ThrowsAsync<DatabaseException>(async () => 
                await transactionRepository.Add(transaction2));
        }

        [Fact]
        public async Task AddTransfer_ShouldSucceed()
        {
            var transactionAddFaker = new Common.Fakers.TransactionAddFaker()
                .RuleFor(ta => ta.AuthUserId, authUserId)
                .RuleFor(ta => ta.AccountId, account.ExternalId)
                .RuleFor(ta => ta.OtherAccountId, otherAccount.ExternalId);

            var transactionAdd = transactionAddFaker.Generate();

            await transactionRepository.Add(transactionAdd);

            var account2 = await dbContext.Accounts.FindAsync(account.Id);
            Assert.Equal(transactionAdd.DateTimeUtc, account2.LastChangeUtc);
            Assert.Equal(account.Credits + transactionAdd.CreditsChange, account2.Credits, 10);

            var transaction = await dbContext.Transactions.Where(t => t.AccountId == account2.Id).OrderByDescending(t => t.Id).FirstAsync();
            Assert.Equal(transactionAdd.CreditsChange, transaction.CreditsChange, 10);
            Assert.Equal(account.Credits + transactionAdd.CreditsChange, transaction.CreditsNew, 10);
            Assert.Equal(transactionAdd.TransactionId, transaction.ExternalId);
            Assert.Equal(transactionAdd.Description, transaction.Description);
            Assert.Equal(transactionAdd.DateTimeUtc, transaction.DateTimeUtc);

            var otherAccount2 = await dbContext.Accounts.FindAsync(otherAccount.Id);
            Assert.Equal(transactionAdd.DateTimeUtc, otherAccount2.LastChangeUtc);
            Assert.Equal(otherAccount.Credits - transactionAdd.CreditsChange, otherAccount2.Credits, 10);

            var otherTransaction = await dbContext.Transactions.Where(t => t.AccountId == otherAccount2.Id).OrderByDescending(t => t.Id).FirstAsync();
            Assert.Equal(-transactionAdd.CreditsChange, otherTransaction.CreditsChange, 10);
            Assert.Equal(otherAccount.Credits - transactionAdd.CreditsChange, otherTransaction.CreditsNew, 10);
            Assert.Equal(transactionAdd.TransactionId, otherTransaction.ExternalId);
            Assert.Equal(transactionAdd.Description, otherTransaction.Description);
            Assert.Equal(transactionAdd.DateTimeUtc, otherTransaction.DateTimeUtc);
            
            Assert.Equal(transaction.Id, otherTransaction.PrimaryTransactionId);
        }

        [Fact]
        public async Task Get_WithInvalidArguments_ShouldFail()
        {
            await Assert.ThrowsAsync<ArgumentException>(async () => 
                await transactionRepository.Get(null, account.ExternalId));
            await Assert.ThrowsAsync<ArgumentException>(async () => 
                await transactionRepository.Get("", account.ExternalId));
            await Assert.ThrowsAsync<ArgumentException>(async () => 
                await transactionRepository.Get(authUserId, account.ExternalId, -1));
            await Assert.ThrowsAsync<ArgumentException>(async () => 
                await transactionRepository.Get(authUserId, account.ExternalId, 0, -1));
        }

        [Fact]
        public async Task Get_WithUnknownAccount_ShouldFail()
        {
            Guid unknownAccountId = Guid.NewGuid();
            await Assert.ThrowsAsync<AccountNotFoundException>(async () => 
                await transactionRepository.Get(authUserId, unknownAccountId));
        }

        [Fact]
        public async Task Get_WithSomebodyElsesAccount_ShouldFail()
        {
            await Assert.ThrowsAsync<AccountNotFoundException>(async () => 
                await transactionRepository.Get(otherAuthUserId, account.ExternalId));
        }

        [Fact]
        public async Task Get_ShouldSucceed()
        {
            var list = await transactionRepository.Get(authUserId, account.ExternalId);
            Assert.Single(list);
            list = await transactionRepository.Get(authUserId, account.ExternalId, 0, 0);
            Assert.Empty(list);
            list = await transactionRepository.Get(authUserId, account.ExternalId, 1, 1);
            Assert.Empty(list);

            list = await transactionRepository.Get(otherAuthUserId, otherAccount.ExternalId);
            Assert.Single(list);
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
