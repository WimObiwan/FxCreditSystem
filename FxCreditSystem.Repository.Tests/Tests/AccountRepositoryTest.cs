using System;
using System.Threading.Tasks;
using AutoMapper;
using FxCreditSystem.Common.Fakers;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FxCreditSystem.Repository.Tests
{
    public class AccountRepositoryTest : Shared.TestBase
    {
        private Guid AccountId => databaseSeeder.Account.ExternalId;
        private Guid OtherAccountId => databaseSeeder.OtherAccount.ExternalId;
        private string UserIdentity => databaseSeeder.User.Identities[0].Identity;
        private string OtherUserIdentity => databaseSeeder.OtherUser.Identities[0].Identity;
        private Entities.Account Account => databaseSeeder.Account;
        private Entities.Account OtherAccount => databaseSeeder.OtherAccount;

        private readonly FxCreditSystem.Repository.AccountRepository accountRepository;

        public AccountRepositoryTest() : base(nameof(AccountRepositoryTest))
        {
            accountRepository = new FxCreditSystem.Repository.AccountRepository(dbContext, mapper);
        }

        [Fact]
        public async Task HasIdentity_ShouldSucceed()
        {
            var result = await accountRepository.HasIdentity(AccountId, UserIdentity);
            Assert.True(result);

            result = await accountRepository.HasIdentity(OtherAccountId, OtherUserIdentity);
            Assert.True(result);
        }

        [Fact]
        public async Task HasIdentity_WithoutIdentity_ShouldSucceed()
        {
            var result = await accountRepository.HasIdentity(AccountId, OtherUserIdentity);
            Assert.False(result);

            result = await accountRepository.HasIdentity(OtherAccountId, UserIdentity);
            Assert.False(result);
        }

        [Fact]
        public async Task HasIdentity_WithUnknownAccount_ShouldFail()
        {
            Bogus.Faker faker = new Bogus.Faker();
            var identity = faker.Random.Identity();
            var accountId = faker.Random.Guid();

            await Assert.ThrowsAsync<AccountNotFoundException>(async () =>
                await accountRepository.HasIdentity(accountId, identity));
        }

        [Fact]
        public async Task GetAccount_WithUnknownUserId_ShouldFail()
        {
            Guid unknownAccountId = Guid.NewGuid();

            await Assert.ThrowsAsync<AccountNotFoundException>(async () =>
                await accountRepository.GetAccount(unknownAccountId));
        }

        [Fact]
        public async Task GetAccount_ShouldSucceed()
        {
            var account = await accountRepository.GetAccount(AccountId);
            Assert.NotNull(account);
            Assert.Equal(AccountId, account.Id);
            Assert.Equal(Account.Description, account.Description);
            Assert.Equal(Account.MinimumCredits, account.MinimumCredits);
            Assert.Equal(Account.Credits, account.Credits);
            Assert.Equal(Account.LastChangeUtc, account.LastChangeUtc);

            account = await accountRepository.GetAccount(OtherAccountId);
            Assert.NotNull(account);
            Assert.Equal(OtherAccountId, account.Id);
            Assert.Equal(OtherAccount.Description, account.Description);
            Assert.Equal(OtherAccount.MinimumCredits, account.MinimumCredits);
            Assert.Equal(OtherAccount.Credits, account.Credits);
            Assert.Equal(OtherAccount.LastChangeUtc, account.LastChangeUtc);
        }
    }
}
