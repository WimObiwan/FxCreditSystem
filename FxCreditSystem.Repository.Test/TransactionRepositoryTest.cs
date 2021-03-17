using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FxCreditSystem.Repository.Test
{
    public class TransactionRepositoryTest : Shared.TestBase
    {
        private string AuthUserId => databaseSeeder.AuthUserId;
        private string OtherAuthUserId => databaseSeeder.OtherAuthUserId;
        private Entities.Account Account => databaseSeeder.Account;
        private Entities.Account OtherAccount => databaseSeeder.OtherAccount;

        private readonly FxCreditSystem.Repository.AccountUserRepository accountUserRepository;
        private readonly FxCreditSystem.Repository.TransactionRepository transactionRepository;

        public TransactionRepositoryTest() : base(nameof(TransactionRepositoryTest))
        {
            accountUserRepository = new FxCreditSystem.Repository.AccountUserRepository(dbContext, mapper);
            transactionRepository = new FxCreditSystem.Repository.TransactionRepository(dbContext, accountUserRepository, mapper);
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
                .RuleFor(ta => ta.AuthUserId, AuthUserId)
                .RuleFor(ta => ta.AccountId, Account.ExternalId)
                .RuleFor(ta => ta.OtherAccountId, Account.ExternalId);

            var transactionAdd = transactionAddFaker.Generate();

            await Assert.ThrowsAsync<TransactionBetweenSameAccountsException>(async () => 
                await transactionRepository.Add(transactionAdd));
        }

        [Fact]
        public async Task AddTransfer_FromUnknownAccount_ShouldFail()
        {
            var transactionAddFaker = new Common.Fakers.TransactionAddFaker()
                .RuleFor(ta => ta.AuthUserId, AuthUserId)
                .RuleFor(ta => ta.OtherAccountId, OtherAccount.ExternalId);

            var transactionAdd = transactionAddFaker.Generate();

            await Assert.ThrowsAsync<AccountNotFoundException>(async () => 
                await transactionRepository.Add(transactionAdd));
        }

        [Fact]
        public async Task AddTransfer_SomebodyElsesAccount_ShouldFail()
        {
            var transactionAddFaker = new Common.Fakers.TransactionAddFaker()
                .RuleFor(ta => ta.AuthUserId, OtherAuthUserId)
                .RuleFor(ta => ta.AccountId, Account.ExternalId)
                .RuleFor(ta => ta.OtherAccountId, OtherAccount.ExternalId);

            var transactionAdd = transactionAddFaker.Generate();

            await Assert.ThrowsAsync<AccountNotFoundException>(async () => 
                await transactionRepository.Add(transactionAdd));
        }

        [Fact]
        public async Task AddTransfer_ToUnknownAccount_ShouldFail()
        {
            var transactionAddFaker = new Common.Fakers.TransactionAddFaker()
                .RuleFor(ta => ta.AuthUserId, AuthUserId)
                .RuleFor(ta => ta.AccountId, Account.ExternalId);

            var transactionAdd = transactionAddFaker.Generate();

            await Assert.ThrowsAsync<AccountNotFoundException>(async () => 
                await transactionRepository.Add(transactionAdd));
        }

        [Fact]
        public async Task AddTransfer_BelowMinimumCredits_ShouldFail()
        {
            var transactionAddFaker = new Common.Fakers.TransactionAddFaker()
                .RuleFor(ta => ta.AuthUserId, AuthUserId)
                .RuleFor(ta => ta.AccountId, Account.ExternalId)
                .RuleFor(ta => ta.OtherAccountId, OtherAccount.ExternalId)
                .RuleFor(ta => ta.CreditsChange, f => f.Random.Decimal(-Account.Credits - 10.0m - 0.01m, -Account.Credits - 10.0m - 50m));

            var transactionAdd = transactionAddFaker.Generate();

            await Assert.ThrowsAsync<AccountCreditsInsufficientException>(async () => 
                await transactionRepository.Add(transactionAdd));
        }

        [Fact]
        public async Task AddTransfer_TakingCreditsFromOtherAccount_ShouldFail()
        {
            var transactionAddFaker = new Common.Fakers.TransactionAddFaker()
                .RuleFor(ta => ta.AuthUserId, AuthUserId)
                .RuleFor(ta => ta.AccountId, Account.ExternalId)
                .RuleFor(ta => ta.OtherAccountId, OtherAccount.ExternalId)
                .RuleFor(ta => ta.CreditsChange, f => f.Random.Decimal(+1m, +50m));

            var transactionAdd = transactionAddFaker.Generate();

            await Assert.ThrowsAsync<DebetFromOtherAccountNotAllowedException>(async () => 
                await transactionRepository.Add(transactionAdd));
        }

        [Fact]
        public async Task AddTransfer_WithSameTransactionId_ShouldFail()
        {
            var transactionAddFaker = new Common.Fakers.TransactionAddFaker()
                .RuleFor(ta => ta.AuthUserId, AuthUserId)
                .RuleFor(ta => ta.AccountId, Account.ExternalId)
                .RuleFor(ta => ta.OtherAccountId, OtherAccount.ExternalId);

            var transaction1 = transactionAddFaker.Generate();
            var transaction2 = transactionAddFaker
                .RuleFor(ta => ta.TransactionId, transaction1.TransactionId)
                .Generate();

            await transactionRepository.Add(transaction1);
            await Assert.ThrowsAsync<DatabaseException>(async () => 
                await transactionRepository.Add(transaction2));
        }

        [Fact]
        public async Task AddTransfer_ShouldSucceed()
        {
            var transactionAddFaker = new Common.Fakers.TransactionAddFaker()
                .RuleFor(ta => ta.AuthUserId, AuthUserId)
                .RuleFor(ta => ta.AccountId, Account.ExternalId)
                .RuleFor(ta => ta.OtherAccountId, OtherAccount.ExternalId);

            var transactionAdd = transactionAddFaker.Generate();
            await transactionRepository.Add(transactionAdd);

            var account2 = await dbContext.Accounts.FindAsync(Account.Id);
            Assert.Equal(transactionAdd.DateTimeUtc, account2.LastChangeUtc);
            Assert.Equal(Account.Credits + transactionAdd.CreditsChange, account2.Credits, 10);

            var transaction = await dbContext.Transactions
                .Where(t => t.AccountId == account2.Id)
                .OrderByDescending(t => t.Id)
                .FirstAsync();
            Assert.Equal(transactionAdd.CreditsChange, transaction.CreditsChange, 10);
            Assert.Equal(Account.Credits + transactionAdd.CreditsChange, transaction.CreditsNew, 10);
            Assert.Equal(transactionAdd.TransactionId, transaction.ExternalId);
            Assert.Equal(transactionAdd.Description, transaction.Description);
            Assert.Equal(transactionAdd.DateTimeUtc, transaction.DateTimeUtc);

            var otherAccount2 = await dbContext.Accounts.FindAsync(OtherAccount.Id);
            Assert.Equal(transactionAdd.DateTimeUtc, otherAccount2.LastChangeUtc);
            Assert.Equal(OtherAccount.Credits - transactionAdd.CreditsChange, otherAccount2.Credits, 10);

            var otherTransaction = await dbContext.Transactions
                .Where(t => t.AccountId == otherAccount2.Id)
                .OrderByDescending(t => t.Id)
                .FirstAsync();
            Assert.Equal(-transactionAdd.CreditsChange, otherTransaction.CreditsChange, 10);
            Assert.Equal(OtherAccount.Credits - transactionAdd.CreditsChange, otherTransaction.CreditsNew, 10);
            Assert.Equal(transactionAdd.TransactionId, otherTransaction.ExternalId);
            Assert.Equal(transactionAdd.Description, otherTransaction.Description);
            Assert.Equal(transactionAdd.DateTimeUtc, otherTransaction.DateTimeUtc);
            
            Assert.Equal(transaction.Id, otherTransaction.PrimaryTransactionId);
        }

        [Fact]
        public async Task Get_WithInvalidArguments_ShouldFail()
        {
            await Assert.ThrowsAsync<ArgumentException>(async () => 
                await transactionRepository.Get(null, Account.ExternalId));
            await Assert.ThrowsAsync<ArgumentException>(async () => 
                await transactionRepository.Get("", Account.ExternalId));
            await Assert.ThrowsAsync<ArgumentException>(async () => 
                await transactionRepository.Get(AuthUserId, Account.ExternalId, -1));
            await Assert.ThrowsAsync<ArgumentException>(async () => 
                await transactionRepository.Get(AuthUserId, Account.ExternalId, 0, -1));
        }

        [Fact]
        public async Task Get_WithUnknownAccount_ShouldFail()
        {
            Guid unknownAccountId = Guid.NewGuid();
            await Assert.ThrowsAsync<AccountNotFoundException>(async () => 
                await transactionRepository.Get(AuthUserId, unknownAccountId));
        }

        [Fact]
        public async Task Get_WithSomebodyElsesAccount_ShouldFail()
        {
            await Assert.ThrowsAsync<AccountNotFoundException>(async () => 
                await transactionRepository.Get(OtherAuthUserId, Account.ExternalId));
        }

        [Fact]
        public async Task Get_ShouldSucceed()
        {
            var list = await transactionRepository.Get(AuthUserId, Account.ExternalId);
            Assert.Single(list);
            list = await transactionRepository.Get(AuthUserId, Account.ExternalId, 0, 0);
            Assert.Empty(list);
            list = await transactionRepository.Get(AuthUserId, Account.ExternalId, 1, 1);
            Assert.Empty(list);

            list = await transactionRepository.Get(OtherAuthUserId, OtherAccount.ExternalId);
            Assert.Single(list);
        }
    }
}
