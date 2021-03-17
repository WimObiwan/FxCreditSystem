using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FxCreditSystem.Repository.Test
{
    public class AccountUserRepositoryTest : Shared.TestBase
    {
        private string AuthUserId => databaseSeeder.AuthUserId;
        private string OtherAuthUserId => databaseSeeder.OtherAuthUserId;
        private Entities.Account Account => databaseSeeder.Account;
        private Entities.Account OtherAccount => databaseSeeder.OtherAccount;

        private readonly FxCreditSystem.Repository.AccountUserRepository accountUserRepository;

        public AccountUserRepositoryTest() : base(nameof(AccountUserRepositoryTest))
        {
            accountUserRepository = new FxCreditSystem.Repository.AccountUserRepository(dbContext, mapper);
        }

        [Fact]
        public async Task Get_WithInvalidArguments_ShouldFail()
        {
            await Assert.ThrowsAsync<ArgumentException>(async () => 
                await accountUserRepository.Get(null));
            await Assert.ThrowsAsync<ArgumentException>(async () => 
                await accountUserRepository.Get(""));
        }

        [Fact]
        public async Task Get_WithUnknownUserId_ShouldFail()
        {
            string unknownUserId = Guid.NewGuid().ToString();

            var list = await accountUserRepository.Get(unknownUserId);
            Assert.Empty(list);
        }

        [Fact]
        public async Task Get_ShouldSucceed()
        {
            var list = await accountUserRepository.Get(AuthUserId);
            var accountUser = Assert.Single(list);
            Assert.Equal(Account.ExternalId, accountUser.AccountExternalId);
            Assert.Equal(Account.Description, accountUser.AccountDescription);
            Assert.Equal(AuthUserId, accountUser.AuthUserId);

            list = await accountUserRepository.Get(OtherAuthUserId);
            accountUser = Assert.Single(list);
            Assert.Equal(OtherAccount.ExternalId, accountUser.AccountExternalId);
            Assert.Equal(OtherAccount.Description, accountUser.AccountDescription);
            Assert.Equal(OtherAuthUserId, accountUser.AuthUserId);
        }
    }
}
