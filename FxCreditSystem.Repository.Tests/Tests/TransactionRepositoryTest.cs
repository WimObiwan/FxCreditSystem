using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FxCreditSystem.Common.Fakers;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FxCreditSystem.Repository.Tests
{
    public class TransactionRepositoryTest : Shared.TestBase
    {
        private Guid UserId => databaseSeeder.User.ExternalId;
        private Guid OtherUserId => databaseSeeder.OtherUser.ExternalId;
        private Entities.Account Account => databaseSeeder.Account;
        private Entities.Account OtherAccount => databaseSeeder.OtherAccount;

        private readonly FxCreditSystem.Repository.UserRepository userRepository;
        private readonly FxCreditSystem.Repository.TransactionRepository transactionRepository;

        public TransactionRepositoryTest() : base(nameof(TransactionRepositoryTest))
        {
            userRepository = new FxCreditSystem.Repository.UserRepository(dbContext, mapper);
            transactionRepository = new FxCreditSystem.Repository.TransactionRepository(dbContext, userRepository, mapper);
        }

        [Fact]
        public async Task AddTransfer_WithInvalidArguments_ShouldFail()
        {
            var addTransactionCommandFaker = new Common.Fakers.AddTransactionCommandFaker();
            await Assert.ThrowsAsync<ArgumentException>(async () => 
                await transactionRepository.Add(
                    addTransactionCommandFaker
                        .RuleFor(ta => ta.CreditsChange, 0m)
                        .Generate()));
        }

        [Fact]
        public async Task AddTransfer_BetweenSameAccounts_ShouldFail()
        {
            var addTransactionCommandFaker = new Common.Fakers.AddTransactionCommandFaker()
                .RuleFor(ta => ta.UserId, UserId)
                .RuleFor(ta => ta.AccountId, Account.ExternalId)
                .RuleFor(ta => ta.OtherAccountId, Account.ExternalId);

            var addTransactionCommand = addTransactionCommandFaker.Generate();

            await Assert.ThrowsAsync<TransactionBetweenSameAccountsException>(async () => 
                await transactionRepository.Add(addTransactionCommand));
        }

        [Fact]
        public async Task AddTransfer_FromUnknownAccount_ShouldFail()
        {
            var addTransactionCommandFaker = new Common.Fakers.AddTransactionCommandFaker()
                .RuleFor(ta => ta.UserId, UserId)
                .RuleFor(ta => ta.OtherAccountId, OtherAccount.ExternalId);

            var addTransactionCommand = addTransactionCommandFaker.Generate();

            await Assert.ThrowsAsync<AccountNotFoundException>(async () => 
                await transactionRepository.Add(addTransactionCommand));
        }

        [Fact]
        public async Task AddTransfer_SomebodyElsesAccount_ShouldFail()
        {
            var addTransactionCommandFaker = new Common.Fakers.AddTransactionCommandFaker()
                .RuleFor(ta => ta.UserId, OtherUserId)
                .RuleFor(ta => ta.AccountId, Account.ExternalId)
                .RuleFor(ta => ta.OtherAccountId, OtherAccount.ExternalId);

            var addTransactionCommand = addTransactionCommandFaker.Generate();

            await Assert.ThrowsAsync<AccountNotFoundException>(async () => 
                await transactionRepository.Add(addTransactionCommand));
        }

        [Fact]
        public async Task AddTransfer_ToUnknownAccount_ShouldFail()
        {
            var addTransactionCommandFaker = new Common.Fakers.AddTransactionCommandFaker()
                .RuleFor(ta => ta.UserId, UserId)
                .RuleFor(ta => ta.AccountId, Account.ExternalId);

            var addTransactionCommand = addTransactionCommandFaker.Generate();

            await Assert.ThrowsAsync<AccountNotFoundException>(async () => 
                await transactionRepository.Add(addTransactionCommand));
        }

        [Fact]
        public async Task AddTransfer_BelowMinimumCredits_ShouldFail()
        {
            var addTransactionCommandFaker = new Common.Fakers.AddTransactionCommandFaker()
                .RuleFor(ta => ta.UserId, UserId)
                .RuleFor(ta => ta.AccountId, Account.ExternalId)
                .RuleFor(ta => ta.OtherAccountId, OtherAccount.ExternalId)
                .RuleFor(ta => ta.CreditsChange, f => 
                    f.Random.Money(-Account.Credits + Account.MinimumCredits - 0.01m, -Account.Credits + Account.MinimumCredits - 50m));

            var addTransactionCommand = addTransactionCommandFaker.Generate();

            await Assert.ThrowsAsync<AccountCreditsInsufficientException>(async () => 
                await transactionRepository.Add(addTransactionCommand));
        }

        [Fact]
        public async Task AddTransfer_TakingCreditsFromOtherAccount_ShouldFail()
        {
            var addTransactionCommandFaker = new Common.Fakers.AddTransactionCommandFaker()
                .RuleFor(ta => ta.UserId, UserId)
                .RuleFor(ta => ta.AccountId, Account.ExternalId)
                .RuleFor(ta => ta.OtherAccountId, OtherAccount.ExternalId)
                .RuleFor(ta => ta.CreditsChange, f => f.Random.Money(+1m, +50m));

            var addTransactionCommand = addTransactionCommandFaker.Generate();

            await Assert.ThrowsAsync<DebetFromOtherAccountNotAllowedException>(async () => 
                await transactionRepository.Add(addTransactionCommand));
        }

        [Fact]
        public async Task AddTransfer_WithSameTransactionId_ShouldFail()
        {
            var addTransactionCommandFaker = new Common.Fakers.AddTransactionCommandFaker()
                .RuleFor(ta => ta.UserId, UserId)
                .RuleFor(ta => ta.AccountId, Account.ExternalId)
                .RuleFor(ta => ta.OtherAccountId, OtherAccount.ExternalId);

            var transaction1 = addTransactionCommandFaker.Generate();
            var transaction2 = addTransactionCommandFaker
                .RuleFor(ta => ta.TransactionId, transaction1.TransactionId)
                .Generate();

            await transactionRepository.Add(transaction1);
            await Assert.ThrowsAsync<DatabaseException>(async () => 
                await transactionRepository.Add(transaction2));
        }

        [Fact]
        public async Task AddTransfer_ShouldSucceed()
        {
            var addTransactionCommandFaker = new Common.Fakers.AddTransactionCommandFaker()
                .RuleFor(ta => ta.UserId, UserId)
                .RuleFor(ta => ta.AccountId, Account.ExternalId)
                .RuleFor(ta => ta.OtherAccountId, OtherAccount.ExternalId);

            var addTransactionCommand = addTransactionCommandFaker.Generate();
            await transactionRepository.Add(addTransactionCommand);

            var account2 = await dbContext.Accounts.FindAsync(Account.Id);
            Assert.Equal(addTransactionCommand.DateTimeUtc, account2.LastChangeUtc);
            Assert.Equal(Account.Credits + addTransactionCommand.CreditsChange, account2.Credits);

            var transaction = await dbContext.Transactions
                .Where(t => t.AccountId == account2.Id)
                .OrderByDescending(t => t.Id)
                .FirstAsync();
            Assert.Equal(addTransactionCommand.CreditsChange, transaction.CreditsChange);
            Assert.Equal(Account.Credits + addTransactionCommand.CreditsChange, transaction.CreditsNew);
            Assert.Equal(addTransactionCommand.TransactionId, transaction.ExternalId);
            Assert.Equal(addTransactionCommand.Description, transaction.Description);
            Assert.Equal(addTransactionCommand.DateTimeUtc, transaction.DateTimeUtc);

            var otherAccount2 = await dbContext.Accounts.FindAsync(OtherAccount.Id);
            Assert.Equal(addTransactionCommand.DateTimeUtc, otherAccount2.LastChangeUtc);
            Assert.Equal(OtherAccount.Credits - addTransactionCommand.CreditsChange, otherAccount2.Credits);

            var otherTransaction = await dbContext.Transactions
                .Where(t => t.AccountId == otherAccount2.Id)
                .OrderByDescending(t => t.Id)
                .FirstAsync();
            Assert.Equal(-addTransactionCommand.CreditsChange, otherTransaction.CreditsChange);
            Assert.Equal(OtherAccount.Credits - addTransactionCommand.CreditsChange, otherTransaction.CreditsNew);
            Assert.Equal(addTransactionCommand.TransactionId, otherTransaction.ExternalId);
            Assert.Equal(addTransactionCommand.Description, otherTransaction.Description);
            Assert.Equal(addTransactionCommand.DateTimeUtc, otherTransaction.DateTimeUtc);
            
            Assert.Equal(transaction.Id, otherTransaction.PrimaryTransactionId);
        }

        [Fact]
        public async Task Get_WithInvalidArguments_ShouldFail()
        {
            await Assert.ThrowsAsync<ArgumentException>(async () => 
                await transactionRepository.Get(UserId, Account.ExternalId, -1));
            await Assert.ThrowsAsync<ArgumentException>(async () => 
                await transactionRepository.Get(UserId, Account.ExternalId, 0, -1));
        }

        [Fact]
        public async Task Get_WithUnknownAccount_ShouldFail()
        {
            Guid unknownAccountId = Guid.NewGuid();
            await Assert.ThrowsAsync<AccountNotFoundException>(async () => 
                await transactionRepository.Get(UserId, unknownAccountId));
        }

        [Fact]
        public async Task Get_WithSomebodyElsesAccount_ShouldFail()
        {
            await Assert.ThrowsAsync<AccountNotFoundException>(async () => 
                await transactionRepository.Get(OtherUserId, Account.ExternalId));
        }

        [Fact]
        public async Task Get_ShouldSucceed()
        {
            var list = await transactionRepository.Get(UserId, Account.ExternalId);
            Assert.Single(list);
            list = await transactionRepository.Get(UserId, Account.ExternalId, 0, 0);
            Assert.Empty(list);
            list = await transactionRepository.Get(UserId, Account.ExternalId, 1, 1);
            Assert.Empty(list);

            list = await transactionRepository.Get(OtherUserId, OtherAccount.ExternalId);
            Assert.Single(list);
        }
    }
}
